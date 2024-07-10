using StringExpressionCalculator.Domain.Application.Operations.Abstractions;
using StringExpressionCalculator.Domain.Application.Operations.Enums;

namespace StringExpressionCalculator.Domain.Application.Operations;

/// <summary>
/// Математическая операция - деление.
/// </summary>
public class DivisionOperation : IMathOperation
{
    public string Symbol => "/";

    public OperationPriority Priority => OperationPriority.High;

    public OperationType Type => OperationType.Binary;

    public double Calculate(double leftOperand, double? rightOperand = null)
    {
        CheckRightOperand(rightOperand);
        return leftOperand / rightOperand.GetValueOrDefault();
    }

    private void CheckRightOperand(double? right)
    {
        ArgumentNullException.ThrowIfNull(right);

        if (right == 0)
            throw new DivideByZeroException();
    }
}