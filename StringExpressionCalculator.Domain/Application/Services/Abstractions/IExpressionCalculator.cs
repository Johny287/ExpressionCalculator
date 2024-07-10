namespace StringExpressionCalculator.Domain.Application.Services.Abstractions;

public interface IExpressionCalculator
{
    double Calculate(string expression);
}