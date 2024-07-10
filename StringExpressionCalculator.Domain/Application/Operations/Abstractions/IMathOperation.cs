using StringExpressionCalculator.Domain.Application.Operations.Enums;

namespace StringExpressionCalculator.Domain.Application.Operations.Abstractions;

/// <summary>
/// Математическая операция.
/// </summary>
public interface IMathOperation
{
    /// <summary>
    /// Символ операции.
    /// </summary>
    string Symbol { get; }

    /// <summary>
    /// Приоритет операции.
    /// </summary>
    OperationPriority Priority { get; }

    /// <summary>
    /// Тип операции.
    /// </summary>
    OperationType Type { get; }

    /// <summary>
    /// Посчитать значение.
    /// Примечание: Если это унарная операция, правый операнд может быть null. 
    /// </summary>
    /// <param name="leftOperand">Левый операнд.</param>
    /// <param name="rightOperand">Правый операнд. Может быть не задан, если это унарная операция!</param>
    /// <returns>Результат операции.</returns>
    double Calculate(double leftOperand, double? rightOperand = null);
}