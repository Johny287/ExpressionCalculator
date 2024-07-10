namespace StringExpressionCalculator.Domain.Application.Operations.Enums;

/// <summary>
/// Приоритет математической операции.
/// </summary>
public enum OperationPriority : byte
{
    Normal,     // + -
    High,       // * /
    Highest,    // любые унарные (например факториал)
}