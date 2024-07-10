using StringExpressionCalculator.Domain.Application.Operations.Abstractions;
using StringExpressionCalculator.Domain.Application.Operations.Enums;

namespace StringExpressionCalculator.Domain.Application.Operations;

/// <summary>
/// Математическая операция - разность.
/// </summary>
public class SubtractionOperation : IMathOperation
{
    public string Symbol => "-";

    public OperationPriority Priority => OperationPriority.Normal;

    public OperationType Type => OperationType.Binary;

    public double Calculate(double leftOperand, double? rightOperand = null)
    {
        ArgumentNullException.ThrowIfNull(rightOperand);
        return leftOperand - rightOperand.Value;
    }
}