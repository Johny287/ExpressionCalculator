using StringExpressionCalculator.Domain.Application.Operations;

namespace StringExpressionCalculator.Tests.Operations;

public class MultiplicationOperationTests
{
    [Theory]
    [InlineData(4, 5, 20)]
    [InlineData(-2.5, 7.8, -19.5)]
    [InlineData(0, -3.14, 0)]
    public void Calculate_ArgumentsAreCorrect_ReturnResult(double left, double right, double expected)
    {
        // Arrange
        var target = new MultiplicationOperation();

        // Act
        var result = target.Calculate(left, right);

        // Asserts
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Calculate_RightArgumentIsNull_Throw()
    {
        // Arrange
        var target = new MultiplicationOperation();

        // Asserts
        Assert.Throws<ArgumentNullException>(() => target.Calculate(2.56, null));
        Assert.Throws<ArgumentNullException>(() => target.Calculate(-8));
    }
}