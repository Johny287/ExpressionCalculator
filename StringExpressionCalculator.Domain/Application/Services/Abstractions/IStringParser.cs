using StringExpressionCalculator.Domain.Application.Models;

namespace StringExpressionCalculator.Domain.Application.Services.Abstractions;

public interface IStringParser
{
    StringOperationContext[] Parse(string bracketsExpression);

    bool IsMarker(string? value);

    int? TryGetIndexFromMarker(string? marker);
}