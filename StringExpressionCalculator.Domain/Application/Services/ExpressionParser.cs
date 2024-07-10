using System.Globalization;
using StringExpressionCalculator.Domain.Application.Models;
using StringExpressionCalculator.Domain.Application.Services.Abstractions;
using StringExpressionCalculator.Domain.Domain;

namespace StringExpressionCalculator.Domain.Application.Services;

/// <summary>
/// Парсер строкового выражения.
/// Операнды преобразуются в тип double.
/// </summary>
public class ExpressionParser : IExpressionParser
{
    private readonly IStringParser _stringParser;

    public ExpressionParser(IStringParser stringParser) => _stringParser = stringParser;

    /// <summary>
    /// Разбить выражение на отдельные простые операции.
    /// Эти операции могут быть как бинарные (с двумя операндами), так и унарные (с одним операндом).
    /// </summary>
    /// <param name="expression">Математическое выражение.</param>
    /// <returns>Список простых операций.</returns>
    public OperationContext[] Parse(string expression)
    {
        var results = new List<OperationContext>();
        
        var contexts = _stringParser.Parse(expression);
        Map(contexts, results);

        return results.ToArray();
    }

    /// <summary>
    /// Конвертировать строковые контексты операций в их аналоги с конкретными типами операндов. 
    /// </summary>
    private void Map(StringOperationContext[] sources, List<OperationContext> targets)
    {
        foreach (var source in sources)
        {
            var target = IsUnary(source) ? MapUnary(source) : MapBinary(targets, source);
            targets.Add(target);
        }
    }

    /// <summary>
    /// Конвертировать унарную операцию.
    /// </summary>
    private OperationContext MapUnary(StringOperationContext context)
    {
        var operand = context.LeftOperand ?? context.RightOperand;
        return new UnaryOperationContext(new Operand(ParseOperand(operand)), context.Symbol);
    }

    /// <summary>
    /// Конвертировать бинарную операцию.
    /// </summary>
    private OperationContext MapBinary(List<OperationContext> results, StringOperationContext context)
    {
        var left = CreateOperand(results, context.LeftOperand);
        var right = CreateOperand(results, context.RightOperand);
        return new BinaryOperationContext(left, right, context.Symbol);
    }

    /// <summary>
    /// Проверить операнды. Если одного нет, значит это должна быть унарная операция (с одним операндом).
    /// </summary>
    private bool IsUnary(StringOperationContext context) => context is { LeftOperand: null } or { RightOperand: null };

    /// <summary>
    /// Создать объект операнда, на основе переданного значения.
    /// Список результатов необходим для ссылки на результат предыдущих операций. 
    /// </summary>
    private Operand CreateOperand(List<OperationContext> results, string? operand)
    {
        if (!_stringParser.IsMarker(operand))
            return new Operand(ParseOperand(operand));

        var index = _stringParser.TryGetIndexFromMarker(operand);
        return index != null ? new Operand(results[index.Value]) : throw new FormatException(operand);
    }

    /// <summary>
    /// Преобразовать строковый операнд в тип double.
    /// </summary>
    private double ParseOperand(string? operand) =>
        double.TryParse(operand?.Replace(',', '.'), CultureInfo.InvariantCulture, out var res) 
            ? res 
            : throw new FormatException(operand);
}