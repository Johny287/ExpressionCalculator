using Moq;
using StringExpressionCalculator.Domain.Application.Models;
using StringExpressionCalculator.Domain.Application.Services;
using StringExpressionCalculator.Domain.Application.Services.Abstractions;
using StringExpressionCalculator.Domain.Domain;

namespace StringExpressionCalculator.Tests;

public class ExpressionParserTests
{
    [Fact]
    public void ParseTest()
    {
        const string exp = "(1 + 2 - 4 / (1 - 2))";

        var stringParserMock = new Mock<IStringParser>();
        stringParserMock.Setup(x => x.Parse(exp)).Returns(new StringOperationContext[] {
            new ("1", "-", "2"),
            new ("4", "/", "{0}"),
            new ("1", "+", "2"),
            new ("{2}", "-", "{1}")
        });
        stringParserMock.Setup(x => x.IsMarker(It.IsIn("{0}", "{1}", "{2}"))).Returns(true);
        stringParserMock.SetupSequence(x => x.TryGetIndexFromMarker(It.IsAny<string>())).Returns(0).Returns(2).Returns(1);
        

        var target = new ExpressionParser(stringParserMock.Object);
        var result = target.Parse(exp);

        Assert.True(result.Length == 4);
        Assert.Equal("-", result[0].Symbol);
        Assert.Equal("/", result[1].Symbol);
        Assert.Equal("-", result[3].Symbol);
        Assert.Equal(1,  (result[2] as BinaryOperationContext)?.GetLeftValue());
    }
}