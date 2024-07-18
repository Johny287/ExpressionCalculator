using StringExpressionCalculator.Domain.Application.Operations.Abstractions;
using StringExpressionCalculator.Domain.Application.Operations.Enums;

namespace StringExpressionCalculator.Domain.Application.Operations;

/// <summary>
/// Математическая операция - степень.
/// </summary>
public class ExponentiationOperation : IMathOperation
{
    public string Symbol => "^";

    public OperationPriority Priority => OperationPriority.Higher;
    
    public OperationType Type => OperationType.Binary;

    public double Calculate(double leftOperand, double? rightOperand = null)
    {
        ArgumentNullException.ThrowIfNull(rightOperand);
        return double.Pow(leftOperand, rightOperand.Value);
    }
}