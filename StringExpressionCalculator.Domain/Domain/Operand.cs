namespace StringExpressionCalculator.Domain.Domain;

/// <summary>
/// Операнд.
/// В нем может храниться как конкретное значение, так и ссылка на результат другой операции.
/// </summary>
public record Operand
{
    private readonly double? _value;
    private readonly OperationContext? _valueResult;

    public Operand(double value) => _value = value;
    public Operand(OperationContext valueResult) => _valueResult = valueResult;

    public double GetValue() => _value ?? _valueResult?.GetResult() ?? throw new ArgumentNullException();
}