namespace StringExpressionCalculator.Domain.Application.Operations.Enums;

/// <summary>
/// Тип математической операции.
/// </summary>
public enum OperationType : byte
{
    Binary, // Бинарная (2 операнда).
    Unary   // Унарная (1 операнд).
}