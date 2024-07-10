using StringExpressionCalculator.Domain.Application.Operations.Abstractions;
using StringExpressionCalculator.Domain.Application.Operations.Enums;

namespace StringExpressionCalculator.Domain.Application.Operations;

/// <summary>
/// Математическая операция - факториал.
/// </summary>
public class FactorialOperation : IMathOperation
{
    public string Symbol => "!";

    public OperationPriority Priority => OperationPriority.Highest;
    
    public OperationType Type => OperationType.Unary;

    public double Calculate(double leftOperand, double? rightOperand = null) 
        => leftOperand switch
        {
            < 0 or double.NaN => throw new ArgumentException(nameof(leftOperand)),
            0 or 1 => 1,
            _ => Enumerable.Range(1, (int)leftOperand).Aggregate((x, y) => x * y)
        };
}