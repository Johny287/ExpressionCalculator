namespace StringExpressionCalculator.Domain.Application.Services.Abstractions;

public interface ICalculator
{
    double CalculateUnary(double operand, string operationSymbol);

    double CalculateBinary(double leftOperand, string operationSymbol, double rightOperand);
}