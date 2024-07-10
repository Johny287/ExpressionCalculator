using StringExpressionCalculator.Domain.Application.Operations;

namespace StringExpressionCalculator.Tests.Operations;

public class FactorialOperationTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(4, 24)]
    [InlineData(11, 39916800)]
    public void Calculate_ArgumentsAreCorrect_ReturnResult(double value, double expected)
    {
        // Arrange
        var target = new FactorialOperation();

        // Act
        var result = target.Calculate(value);

        // Asserts
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Calculate_ArgumentIsLessThenZero_Throw()
    {
        // Arrange
        var target = new FactorialOperation();

        // Asserts
        Assert.ThrowsAny<Exception>(() => target.Calculate(-2));
        Assert.ThrowsAny<Exception>(() => target.Calculate(-8.67));
    }
}