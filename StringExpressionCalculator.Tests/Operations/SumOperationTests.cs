using StringExpressionCalculator.Domain.Application.Operations;

namespace StringExpressionCalculator.Tests.Operations;

public class SumOperationTests
{
    [Theory]
    [InlineData(4, 5, 9)]
    [InlineData(-2.5, 7.8, 5.3)]
    [InlineData(0, -3.14, -3.14)]
    public void Calculate_ArgumentsAreCorrect_ReturnResult(double left, double right, double expected)
    {
        // Arrange
        var target = new SumOperation();

        // Act
        var result = target.Calculate(left, right);

        // Asserts
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Calculate_RightArgumentIsNull_Throw()
    {
        // Arrange
        var target = new SumOperation();

        // Asserts
        Assert.Throws<ArgumentNullException>(() => target.Calculate(2.56, null));
        Assert.Throws<ArgumentNullException>(() => target.Calculate(-8));
    }
}