using StringExpressionCalculator.Domain.Application.Operations;

namespace StringExpressionCalculator.Tests.Operations;

public class ExponentiationOperationTests
{
    [Theory]
    [InlineData(2, 3, 8)]
    [InlineData(2, -3, 0.125)]
    [InlineData(-2, -4, 0.0625)]
    [InlineData(5.5, 0, 1)]
    [InlineData(-3.5, 3, -42.875)]
    public void Calculate_ArgumentsAreCorrect_ReturnResult(double left, double right, double expected)
    {
        // Arrange
        var target = new ExponentiationOperation();

        // Act
        var result = target.Calculate(left, right);

        // Asserts
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Calculate_RightArgumentIsNull_Throw()
    {
        // Arrange
        var target = new ExponentiationOperation();

        // Asserts
        Assert.Throws<ArgumentNullException>(() => target.Calculate(2.56, null));
        Assert.Throws<ArgumentNullException>(() => target.Calculate(-8));
    }
}