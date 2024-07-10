using StringExpressionCalculator.Domain.Application.Operations.Abstractions;
using StringExpressionCalculator.Domain.Application.Operations.Enums;
using StringExpressionCalculator.Domain.Application.Services.Abstractions;

namespace StringExpressionCalculator.Domain.Application.Services;

public class OperationsProvider : IOperationsProvider
{
    private readonly IEnumerable<IMathOperation> _operations;
    private readonly string[] _operationsSymbolsCache;

    public OperationsProvider(IEnumerable<IMathOperation> operations)
    {
        _operations = operations;

        // Символы операций нужны часто, поэтому лучше их закэшировать.
        _operationsSymbolsCache = _operations.Select(op => op.Symbol).ToArray();
    }

    public IMathOperation? GetOperationBySymbol(string operationSymbol) 
        => _operations.SingleOrDefault(op => op.Symbol == operationSymbol);

    public string[] GetAllOperationsSymbols() => _operationsSymbolsCache;

    public string[] GetOperationsSymbolsByPriority(OperationPriority priority)
        => _operations.Where(op => op.Priority == priority).Select(op => op.Symbol).ToArray();
}