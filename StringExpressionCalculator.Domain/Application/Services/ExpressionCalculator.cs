using StringExpressionCalculator.Domain.Application.Services.Abstractions;
using StringExpressionCalculator.Domain.Domain;

namespace StringExpressionCalculator.Domain.Application.Services;

/// <summary>
/// Калькулятор строковых математических выражений.
/// </summary>
public class ExpressionCalculator : IExpressionCalculator
{
    private readonly IExpressionParser _parser;
    private readonly ICalculator _calculator;

    public ExpressionCalculator(IExpressionParser parser, ICalculator calculator)
    {
        _parser = parser;
        _calculator = calculator;
    }

    /// <summary>
    /// Посчитать результат строкового математического выражения.
    /// </summary>
    /// <param name="expression">Математическое выражение. Может содержать скобки.</param>
    /// <returns>Результат.</returns>
    public double Calculate(string expression)
    {
        var operationContexts = _parser.Parse(expression);
        foreach (var operationContext in operationContexts)
        {
            var calculateResult = operationContext switch
            {
                UnaryOperationContext unary 
                    => _calculator.CalculateUnary(unary.GetLeftValue(), unary.Symbol),
                BinaryOperationContext binary 
                    => _calculator.CalculateBinary(binary.GetLeftValue(), binary.Symbol, binary.GetRightValue()),
                _ => throw new NotImplementedException()
            };

            operationContext.SetResult(calculateResult);
        }

        // Финальный результат будет храниться в последнем элементе списка.
        return operationContexts.Last().GetResult() ?? throw new ArgumentNullException();
    }
}