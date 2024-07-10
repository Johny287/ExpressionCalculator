using StringExpressionCalculator.Domain.Application.Operations;

namespace StringExpressionCalculator.Tests.Operations;

public class DivisionOperationTests
{
    [Theory]
    [InlineData(4, 5, 0.8)]
    [InlineData(-2.5, 7.8, -0.32)]
    [InlineData(0, -3.14, 0)]
    public void Calculate_ArgumentsAreCorrect_ReturnResult(double left, double right, double expected)
    {
        // Arrange
        var target = new DivisionOperation();

        // Act
        var result = Math.Round(target.Calculate(left, right), 2);

        // Asserts
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Calculate_RightArgumentIsNull_Throw()
    {
        // Arrange
        var target = new DivisionOperation();

        // Asserts
        Assert.Throws<ArgumentNullException>(() => target.Calculate(2.56, null));
        Assert.Throws<ArgumentNullException>(() => target.Calculate(-8));
    }

    [Fact]
    public void Calculate_RightArgumentIsZero_Throw()
    {
        // Arrange
        var target = new DivisionOperation();

        // Asserts
        Assert.Throws<DivideByZeroException>(() => target.Calculate(2.56, 0));
        Assert.Throws<DivideByZeroException>(() => target.Calculate(-8, 0.0));
    }
}