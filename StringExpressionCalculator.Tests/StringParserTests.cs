using Moq;
using StringExpressionCalculator.Domain.Application.Models;
using StringExpressionCalculator.Domain.Application.Operations.Enums;
using StringExpressionCalculator.Domain.Application.Services;
using StringExpressionCalculator.Domain.Application.Services.Abstractions;

namespace StringExpressionCalculator.Tests;

public class StringParserTests
{
    private readonly StringParser _target;

    public StringParserTests()
    {
        var providerMock = new Mock<IOperationsProvider>(/*MockBehavior.Strict*/);
        providerMock.Setup(x => x.GetAllOperationsSymbols()).Returns(["+", "-", "*", "/", "!"]);
        providerMock.Setup(x => x.GetOperationsSymbolsByPriority(OperationPriority.Normal)).Returns(["+", "-"]);
        providerMock.Setup(x => x.GetOperationsSymbolsByPriority(OperationPriority.High)).Returns(["*", "/"]);
        providerMock.Setup(x => x.GetOperationsSymbolsByPriority(OperationPriority.Highest)).Returns(["!"]);

        _target = new StringParser(providerMock.Object);
    }


    [Theory]
    [MemberData(nameof(DataForParse))]
    public void Parse_IsExpressionCorrect_ReturnCorrect(string expression, (string?, string, string?)[] expected)
    {
        // Act
        var result = _target.Parse(expression);

        // Asserts
        Assert.True(result.Length == expected.Length);
        Assert.Collection(result, expected.Select(exp => new Action<StringOperationContext>(actual =>
            {
                var (left, symbol, right) = actual;
                Assert.Equal(exp, (left, symbol, right));
            })).ToArray());
    }

    public static IEnumerable<object[]> DataForParse =>
    [
        ["-12.21", new[]
        {
            ("-12.21", "+", "0")
        }],
        ["-6 / 3", new[]
        {
            ("-6", "/", "3")
        }],
        ["-2 * 3 + 6 / 3 + 6", new[]
        {
            ("-2", "*", "3"),
            ("6", "/", "3"),
            ("{0}", "+", "{1}"),
            ("{2}", "+", "6")
        }],
        ["(1 + 2 - 4 / (1 - 2)) - 2", new[]
        {
            ("1", "-", "2"),
            ("4", "/", "{0}"),
            ("1", "+", "2"),
            ("{2}", "-", "{1}"),
            ("{3}", "-", "2")
        }],
        ["-23.56 + 2 * (-4.5 - (-8))", new[]
        {
            ("-8", "+", "0"),
            ("-4.5", "-", "{0}"),
            ("2", "*", "{1}"),
            ("-23.56", "+", "{2}")
        }],
        ["6! + 9", new[]
        {
            ("6", "!", null),
            ("{0}", "+", "9")
        }],
        ["!6 + 9", new[]
        {
            (null, "!", "6"),
            ("{0}", "+", "9")
        }],
        ["-9 + 6!", new[]
        {
            ("6", "!", null),
            ("-9", "+", "{0}")
        }],
    ];

    [Theory]
    [MemberData(nameof(DataForSeparateExpression))]
    public void SeparateExpression_IsExpressionCorrect_ReturnCorrect(string expression, (string?, string, string?)[] expected)
    {
        // Act
        var result = _target.SeparateExpression(expression, 0);

        // Asserts
        Assert.True(result.Length == expected.Length);
        Assert.Collection(result, expected.Select(exp => new Action<StringOperationContext>(actual =>
            {
                var (left, symbol, right) = actual;
                Assert.Equal(exp, (left, symbol, right));
            })).ToArray());
    }

    public static IEnumerable<object[]> DataForSeparateExpression =>
    [
        ["-12.21", new[]
        {
            ("-12.21", "+", "0")
        }],
        ["-2 * 3 + 6 / 3 + 6", new[]
        {
            ("-2", "*", "3"),
            ("6", "/", "3"),
            ("{0}", "+", "{1}"),
            ("{2}", "+", "6")
        }],
        ["5.67 - 6.2 * 2 / 3.777", new[]
        {
            ("6.2", "*", "2"),
            ("{0}", "/", "3.777"),
            ("5.67", "-", "{1}")
        }],
        ["!6", new[]
        {
            (null as string, "!", "6")
        }],
        ["-6 / 3", new[]
        {
            ("-6", "/", "3")
        }],
        ["6! + 9", new[]
        {
            ("6", "!", null),
            ("{0}", "+", "9")
        }],
        ["!6 + 9", new[]
        {
            (null, "!", "6"),
            ("{0}", "+", "9")
        }],
        ["-9 + 6!", new[]
        {
            ("6", "!", null),
            ("-9", "+", "{0}")
        }],
    ];

    [Theory]
    [InlineData("(1 + 2)")]
    [InlineData("1 + 2 * (-4)")]
    [InlineData("-2.4 / (-2.19)")]
    public void SeparateExpression_WhenExpressionContainsBrackets_Throw(string expression)
    {
        Assert.ThrowsAny<Exception>(() => _target.SeparateExpression(expression, 0));
    }


    [Theory]
    [InlineData("{5}", true)]
    [InlineData("{456} ", true)]
    [InlineData("{ 2}", false)]
    [InlineData("{t23}", false)]
    [InlineData("{3b}", false)]
    [InlineData("{_}", false)]
    public void IsMarker_Tests(string input, bool expected)
    {
        var result = _target.IsMarker(input);
        Assert.Equal(expected, result);
    }


    [Theory]
    [InlineData("{5}", 5)]
    [InlineData("{456}", 456)]
    public void TryGetIndexFromMarker_IsValueCorrect_ReturnCorrect(string input, int expected)
    {
        var result = _target.TryGetIndexFromMarker(input);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("5")]
    [InlineData("{456")]
    [InlineData("{}")]
    public void TryGetIndexFromMarker_IsValueIncorrect_ReturnNull(string input)
    {
        var result = _target.TryGetIndexFromMarker(input);
        Assert.Null(result);
    }
}
