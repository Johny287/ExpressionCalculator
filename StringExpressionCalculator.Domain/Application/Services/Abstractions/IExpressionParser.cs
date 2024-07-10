using StringExpressionCalculator.Domain.Domain;

namespace StringExpressionCalculator.Domain.Application.Services.Abstractions;

public interface IExpressionParser
{
    OperationContext[] Parse(string expression);
}