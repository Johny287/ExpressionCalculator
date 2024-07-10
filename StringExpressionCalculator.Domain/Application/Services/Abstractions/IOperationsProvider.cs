using StringExpressionCalculator.Domain.Application.Operations.Abstractions;
using StringExpressionCalculator.Domain.Application.Operations.Enums;

namespace StringExpressionCalculator.Domain.Application.Services.Abstractions;

public interface IOperationsProvider
{
    IMathOperation? GetOperationBySymbol(string operationSymbol);

    string[] GetAllOperationsSymbols();

    string[] GetOperationsSymbolsByPriority(OperationPriority priority);
}