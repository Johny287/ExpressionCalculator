using System.Text.RegularExpressions;

namespace StringExpressionCalculator.Domain.Application.Models;

/// <summary>
/// Контекст операции. Содержит символ операции и 2 операнда.
/// Примечания:
/// 1) Есть конструктор, принимающий только один операнд. В этом случае он будет преобразован в операцию суммы с нулем.
/// 2) Это может быть унарная операция (с одним операндом). Тогда второй операнд будет равен null.
///    Причем не важно, с какой стороны от знака операции он находится.
/// 3) Оба операнда, в любом случае, не могут быть пустыми или null.
/// </summary>
public record StringOperationContext
{
    public string? LeftOperand { get; }
    public string? RightOperand { get; }
    public string Symbol { get; }

    public StringOperationContext(string operand) : this(operand, "+", "0") { }

    public StringOperationContext(string? leftOperand, string symbol, string? rightOperand)
    {
        var left = string.IsNullOrWhiteSpace(leftOperand) ? null : leftOperand.Trim();
        var right = string.IsNullOrWhiteSpace(rightOperand) ? null : rightOperand.Trim();

        CheckArguments(left, right);
        CheckSymbol(symbol);

        Symbol = symbol.Trim();
        LeftOperand = left;
        RightOperand = right;
    }

    private void CheckArguments(string? left, string? right)
    {
        if (left == null && right == null)
            throw new ArgumentNullException($"{nameof(left)} and {nameof(right)}");
    }

    private void CheckSymbol(string symbol)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(symbol);

        // Символ операции не должен содержать цифр.
        if (Regex.IsMatch(symbol, @"(\d)+"))
            throw new ArgumentException(symbol);
    }

    public void Deconstruct(out string? leftOperand, out string symbol, out string? rightOperand) 
        => (leftOperand, symbol, rightOperand) = (LeftOperand, Symbol, RightOperand);
}