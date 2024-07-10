using System.Text;
using System.Text.RegularExpressions;
using StringExpressionCalculator.Domain.Application.Models;
using StringExpressionCalculator.Domain.Application.Operations.Enums;
using StringExpressionCalculator.Domain.Application.Services.Abstractions;

namespace StringExpressionCalculator.Domain.Application.Services;

/// <summary>
/// Парсер строкового выражения.
/// Типы операндов не имеют значение и могут быть любыми.
/// Парсинг происходит с учетом символов операций и скобок.
/// Для указания на результат другой операций, используется специальный маркер, который содержит индекс необходимого выражения.
/// </summary>
public class StringParser : IStringParser
{
    private const char OpenBracket = '(';
    private const char CloseBracket = ')';
    private const char MarkerOpenBracket = '{';
    private const char MarkerCloseBracket = '}';

    private readonly IOperationsProvider _operationsProvider;
    private readonly char[] _acceptableSymbols;

    public StringParser(IOperationsProvider operationsProvider)
    {
        _operationsProvider = operationsProvider;

        _acceptableSymbols = _operationsProvider.GetAllOperationsSymbols().Select(char.Parse)
            .Union([OpenBracket, CloseBracket]).Union([',', '.']).Union([' ']).ToArray();
    }

    /// <summary>
    /// Проверить выражение.
    /// Должно содержать только допустимые символы и пробелы. В противном случае выбрасывается исключение.
    /// </summary>
    private void CheckExpression(string expression)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(expression);

        if (expression.Any(x => !char.IsDigit(x) && !_acceptableSymbols.Contains(x)))
            throw new FormatException(expression);
    }

    /// <summary>
    /// Проверить, является ли значение маркером.
    /// Маркер - это индекс(ссылка) на выражение, содержащее результат.
    /// </summary>
    public bool IsMarker(string? value) => value != null && Regex.IsMatch(value.Trim(), @"^{(\d)+}$");

    /// <summary>
    /// Получить индекс из маркера.
    /// </summary>
    public int? TryGetIndexFromMarker(string? marker)
        => IsMarker(marker) && int.TryParse(marker?.Trim()[1..^1], out var res) ? res : null;

    /// <summary>
    /// Создать маркер на основе указанного индекса.
    /// </summary>
    private string CreateMarker(int index) => $"{MarkerOpenBracket}{index}{MarkerCloseBracket}";

    /// <summary>
    /// Разбить строковое выражение на отдельные простые операции.
    /// Выражение может содержать скобки.
    /// </summary>
    public StringOperationContext[] Parse(string bracketsExpression)
    {
        CheckExpression(bracketsExpression);

        var results = new List<StringOperationContext>();
        var sbExpr = new StringBuilder(bracketsExpression);

        var expression = ExtractFirstExpressionInBrackets(sbExpr.ToString());
        while (expression != null)
        {
            AddExpressionToResults(results, expression);
            ReplaceBracketsWithMarker(sbExpr, expression, results.Count - 1);
            expression = ExtractFirstExpressionInBrackets(sbExpr.ToString());
        }

        // Если в конце остался только маркер - не надо заносить его в список результатов!
        if (sbExpr.ToString() is var expr && !IsMarker(expr))
            AddExpressionToResults(results, expr);

        return results.ToArray();
    }

    /// <summary>
    /// Добавить выражение в коллекцию результатов, предварительно разбив его на простые операции.
    /// </summary>
    private void AddExpressionToResults(List<StringOperationContext> results, string expression)
    {
        var strOperationContexts = SeparateExpression(expression, results.Count);
        results.AddRange(strOperationContexts);
    }

    /// <summary>
    /// Найти и вырезать первое, по приоритету, выражение в скобках.
    /// Если выражений в скобках не найдено, вернется null.
    /// </summary>
    private string? ExtractFirstExpressionInBrackets(string bracketsExpression)
    {
        var openBracketIndex = bracketsExpression.LastIndexOf(OpenBracket);
        if (openBracketIndex == -1)
            return null;

        var closeBracketIndex = bracketsExpression.IndexOf(CloseBracket, openBracketIndex);
        return bracketsExpression.Substring(openBracketIndex + 1, closeBracketIndex - openBracketIndex - 1);
    }

    /// <summary>
    /// Заменить указанное выражение в скобках на маркер(индекс). Если таких скобок несколько, заменены будут все. 
    /// </summary>
    private void ReplaceBracketsWithMarker(StringBuilder bracketsExpression, string expression, int markerIndex)
    {
        var oldValue = $"{OpenBracket}{expression}{CloseBracket}";
        var newValue = CreateMarker(markerIndex);
        bracketsExpression.Replace(oldValue, newValue);
    }

    /// <summary>
    /// Разбить строковое выражение на отдельные простые операции.
    /// Выражение не должно содержать скобок!</summary>
    /// <param name="expressionWithoutBrackets">Математическое выражение. Не должно содержать скобок!</param>
    /// <param name="startIndexMarker">Начальный индекс маркера.</param>
    /// <returns>Список простых операций. Содержит маркеры в операндах.</returns>
    public StringOperationContext[] SeparateExpression(string expressionWithoutBrackets, int startIndexMarker)
    {
        var expression = expressionWithoutBrackets.Trim();

        if (IsBracketsExpression(expression))
            throw new FormatException(expression);

        var resultList = new List<StringOperationContext>();

        // Очистить выражение от пробелов и добавить их только между знаками операций.
        // Это необходимо для более удобного разбиения выражения на отдельные элементы.
        expression = AddSpacesBetweenOperationSymbols(expression);

        // Здесь не нужно удалять пустые элементы из массива. Это необходимо для корректного вырезания унарных операций.
        var splitExpression = expression.Split([" "], StringSplitOptions.TrimEntries).ToList();

        // Если выражение представляет из себя только отдельный операнд (например отрицательное число в скобках).
        var operationIndex = FindFirstOperationIndexByPriority(splitExpression);
        if (operationIndex == -1)
            return [CreateOperationContext(splitExpression[0])];

        while (operationIndex != -1)
        {
            var operationContext = ExtractOperationContextByOperationIndex(splitExpression, operationIndex);
            resultList.Add(operationContext);
            ReplaceOperationContextWithMarker(splitExpression, operationIndex, startIndexMarker);
            startIndexMarker++;
            operationIndex = FindFirstOperationIndexByPriority(splitExpression);
        }

        return resultList.ToArray();
    }

    /// <summary>
    /// Найти индекс первой операции с учетом приоритета.
    /// </summary>
    /// <param name="splitExpression">Выражение, разбитое на отдельные элементы.</param>
    /// <returns>Индекс первой приоритетной операции.</returns>
    private int FindFirstOperationIndexByPriority(List<string> splitExpression)
    {
        foreach (var priority in Enum.GetValues<OperationPriority>().OrderByDescending(priority => priority))
        {
            var symbols = _operationsProvider.GetOperationsSymbolsByPriority(priority);
            var operationIndex = splitExpression.FindIndex(value => symbols.Contains(value));
            if (operationIndex != -1)
                return operationIndex;
        }

        return -1;
    }

    /// <summary>
    /// Вырезать контекст простого бинарного/унарного выражения.
    /// <param name="splitExpression">Выражение, разбитое на отдельные элементы.</param>
    /// <param name="operationIndex">Индекс операции, по которому вырезается выражение.</param>
    /// <returns>Вырезанный контекст математической операции.</returns>
    /// </summary>
    private StringOperationContext ExtractOperationContextByOperationIndex(List<string> splitExpression, int operationIndex) 
        => CreateOperationContext(splitExpression[operationIndex - 1], splitExpression[operationIndex], splitExpression[operationIndex + 1]);

    /// <summary>
    /// Заменить выражение на маркер, по указанному индексу операции.
    /// Левый и правый операнды, а также символ операции, будут заменены на маркер.
    /// </summary>
    private void ReplaceOperationContextWithMarker(List<string> splitExpression, int operationIndex, int markerIndex)
    {
        var marker = CreateMarker(markerIndex);
        var startExpressionIndex = operationIndex - 1;
        const int deleteCount = 3; // Всегда 3 элемента. Даже если это унарная операция (3-й элемент - пустота).

        splitExpression.RemoveRange(startExpressionIndex, deleteCount);
        splitExpression.Insert(startExpressionIndex, marker);
    }

    /// <summary>
    /// Создать контекст операции (операнды и символ операции).
    /// Если это унарная операция, один из операндов может быть null, причем не важно какой.
    /// </summary>
    private StringOperationContext CreateOperationContext(string? leftOperand, string operation, string? rightOperand)
        => new(leftOperand, operation, rightOperand);

    /// <summary>
    /// Создать контекст операции (операнды и символ операции).
    /// Если имеется только один операнд, без знака операции (например это отрицательное число в скобках), то будет
    /// создан контекст по умолчанию. То есть он преобразуется в операцию с нулем.
    /// </summary>
    private StringOperationContext CreateOperationContext(string operand) => new(operand);

    /// <summary>
    /// Содержит ли выражение скобки.
    /// </summary>
    private bool IsBracketsExpression(string expression) 
        => expression.Any(x => x == OpenBracket) && expression.Any(x => x == CloseBracket);
    
    /// <summary>
    /// Очистить выражение от пробелов и добавить их только между знаками операций.
    /// </summary>
    private string AddSpacesBetweenOperationSymbols(string expression)
    {
        var sbExpr = new StringBuilder(expression);
        sbExpr.Replace(" ", "");

        // Пропускаем первый символ, если это знак "минус". Его не нужно отделять от числа.
        var skipCount = sbExpr[0] == '-' ? 1 : 0;
        foreach (var symbol in _operationsProvider.GetAllOperationsSymbols())
        {
            sbExpr.Replace(symbol, $" {symbol} ", skipCount, sbExpr.Length - skipCount);
        }

        return sbExpr.ToString();
    }
}