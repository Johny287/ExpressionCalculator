namespace StringExpressionCalculator.Domain.Domain;

/// <summary>
/// Контекст бинарной математической операции. 
/// </summary>
public class BinaryOperationContext : OperationContext
{
    private readonly Operand _leftOperand;
    private readonly Operand _rightOperand;

    public BinaryOperationContext(Operand leftOperand, Operand rightOperand, string symbol) : base(symbol)
    {
        _leftOperand = leftOperand;
        _rightOperand = rightOperand;
    }

    public double GetLeftValue() => _leftOperand.GetValue();
    public double GetRightValue() => _rightOperand.GetValue();
}