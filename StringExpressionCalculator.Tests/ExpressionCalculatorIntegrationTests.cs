using StringExpressionCalculator.Domain.Application.Operations;
using StringExpressionCalculator.Domain.Application.Operations.Abstractions;
using StringExpressionCalculator.Domain.Application.Services.Abstractions;
using StringExpressionCalculator.Domain.Application.Services;

namespace StringExpressionCalculator.Tests;

public class ExpressionCalculatorIntegrationTests : IClassFixture<CalculatorDataFixture>
{
    private readonly IExpressionCalculator _target;

    public ExpressionCalculatorIntegrationTests(CalculatorDataFixture calculatorData) 
        => _target = calculatorData.Target;

    [Theory]
    [InlineData("1 + 2 - 4 / (1 - 2)", 7)]
    [InlineData("(-2 * 4.5 - (24 / 2 ^ 4)) + 4!", 13.5)]
    [InlineData("(-3.14 - (-8.25) + (3.5 ^ 2 / 5 + 3!)) - 3.14", 10.42)]
    public void CalculateTests(string expression, double expectedResult)
    {
        var result = double.Round(_target.Calculate(expression), 3);
        Assert.Equal(expectedResult, result);
    }
}

public class CalculatorDataFixture
{
    public IExpressionCalculator Target { get; }

    public CalculatorDataFixture()
    {
        IMathOperation[] operations =
        [
            new SumOperation(), 
            new SubtractionOperation(), 
            new MultiplicationOperation(), 
            new DivisionOperation(),
            new ExponentiationOperation(),
            new FactorialOperation()
        ];

        var provider = new OperationsProvider(operations);
        var calculator = new Calculator(provider);
        var stringParser = new StringParser(provider);
        var expressionParser = new ExpressionParser(stringParser);

        Target = new ExpressionCalculator(expressionParser, calculator);
    }
}