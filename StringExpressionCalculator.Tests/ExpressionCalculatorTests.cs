using Moq;
using StringExpressionCalculator.Domain.Application.Services;
using StringExpressionCalculator.Domain.Application.Services.Abstractions;
using StringExpressionCalculator.Domain.Domain;

namespace StringExpressionCalculator.Tests;

public class ExpressionCalculatorTests
{
    [Fact]
    public void CalculateTest()
    {
        // Arrange
        const string exp = "1 + 2 - 4 / (1 - 2)";

        var r1 = new BinaryOperationContext(new Operand(1), new Operand(2), "-");
        var r2 = new BinaryOperationContext(new Operand(4), new Operand(r1), "/");
        var r3 = new BinaryOperationContext(new Operand(1), new Operand(2), "+");
        var r4 = new BinaryOperationContext(new Operand(r3), new Operand(r2), "-");
        var parserMock = Mock.Of<IExpressionParser>(x => x.Parse(exp) == new OperationContext[] { r1, r2, r3, r4 });

        var calculatorMock = new Mock<ICalculator>();
        calculatorMock.SetupSequence(x => x.CalculateBinary(It.IsAny<double>(), It.IsAny<string>(), It.IsAny<double>()))
            .Returns(-1).Returns(-4).Returns(3).Returns(7);

        var target = new ExpressionCalculator(parserMock, calculatorMock.Object);

        // Act
        var result = target.Calculate(exp);

        // Asserts
        Assert.Equal(7, result);
    }
}