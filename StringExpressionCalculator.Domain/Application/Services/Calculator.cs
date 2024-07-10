using StringExpressionCalculator.Domain.Application.Operations.Abstractions;
using StringExpressionCalculator.Domain.Application.Services.Abstractions;

namespace StringExpressionCalculator.Domain.Application.Services;

/// <summary>
/// Калькулятор простых бинарных и унарных операций.
/// </summary>
public class Calculator : ICalculator
{
    private readonly IOperationsProvider _operationsProvider;

    public Calculator(IOperationsProvider operationsProvider) => _operationsProvider = operationsProvider;

    /// <summary>
    /// Посчитать унарную операцию.
    /// </summary>
    /// <param name="operand">Значение.</param>
    /// <param name="operationSymbol">Символ операции.</param>
    /// <returns>Результат.</returns>
    public double CalculateUnary(double operand, string operationSymbol) 
        => GetOperation(operationSymbol).Calculate(operand);

    /// <summary>
    /// Посчитать бинарную операцию.
    /// </summary>
    /// <param name="leftOperand">Левый операнд.</param>
    /// <param name="operationSymbol">Символ операции.</param>
    /// <param name="rightOperand">Правый операнд.</param>
    /// <returns>Результат.</returns>
    public double CalculateBinary(double leftOperand, string operationSymbol, double rightOperand) 
        => GetOperation(operationSymbol).Calculate(leftOperand, rightOperand);

    private IMathOperation GetOperation(string operationSymbol)
        => _operationsProvider.GetOperationBySymbol(operationSymbol) ?? throw new NotImplementedException(operationSymbol);
}