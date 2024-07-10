namespace StringExpressionCalculator.Domain.Domain;

/// <summary>
/// Контекст унарной математической операции. 
/// </summary>
public class UnaryOperationContext : OperationContext
{
    private readonly Operand _operand;

    public UnaryOperationContext(Operand operand, string symbol) : base(symbol)
        => _operand = operand;

    public double GetLeftValue() => _operand.GetValue();
}