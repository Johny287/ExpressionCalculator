using Moq;
using StringExpressionCalculator.Domain.Application.Models;
using StringExpressionCalculator.Domain.Application.Operations.Abstractions;
using StringExpressionCalculator.Domain.Application.Services;
using StringExpressionCalculator.Domain.Application.Services.Abstractions;

namespace StringExpressionCalculator.Tests;

public class StringParserOldTests
{
    private readonly StringParserOld _target;

    // Перед каждым тестом в этом классе.
    public StringParserOldTests()
    {
        var providerMock = new Mock<IOperationsProvider>(MockBehavior.Strict);
        providerMock.Setup(x => x.GetAllOperationsSymbols()).Returns(["+", "-", "*", "/", "!"]);
        // providerMock.Setup(x => x.GetOperationsSymbolsByPriority(It.IsAny<OperationPriority>())).Returns(["+", "-", "*", "/"]);
        providerMock.Setup(x => x.GetOperationsSymbolsByPriority(OperationPriority.Normal)).Returns(["+", "-"]);
        providerMock.Setup(x => x.GetOperationsSymbolsByPriority(OperationPriority.High)).Returns(["*", "/"]);
        providerMock.Setup(x => x.GetOperationsSymbolsByPriority(OperationPriority.Highest)).Returns(["!"]);

        _target = new StringParserOld(providerMock.Object);
    }

    [Theory]
    [InlineData("{5}", 5)]
    [InlineData("{456}", 456)]
    public void GetIndex(string input, int expected)
    {
        // var target = new StringParser();

        var result = _target.TryGetIndexFromMarker(input);

        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("{5}", true)]
    [InlineData("{456} ", true)]
    [InlineData("{ 2}", false)]
    [InlineData("{t23}", false)]
    [InlineData("{3b}", false)]
    [InlineData("{_}", false)]
    public void IsMarkerTests(string input, bool expected)
    {
        // var target = new StringParser();

        var result = _target.IsMarker(input);

        Assert.Equal(expected, result);
    }

    [Fact]
    public void Check()
    {
        // var target = new StringParser();

        _target.Check("-23.56 + 2 * (-4.5 - (-8))");

        // Assert.Equal(expected, result);
    }


    [Theory]
    [MemberData(nameof(DataForParse))]
    // public void Parse_IsExpressionCorrect_Return(string expression, string[] expected)
    public void Parse_IsExpressionCorrect_Return(string expression, StrOperationContextBase[] expected)
    {
        // Arrange
        // var target = new StringParser(null);

        // Act
        var result = _target.Parse(expression);

        // Asserts
        Assert.True(result.Length == expected.Length);
        Assert.Collection(result, expected.Select(exp => new Action<StrOperationContextBase>(actual => Assert.Equal(exp, actual))).ToArray());

        // Assert.True(result.Length == 4);
        // Assert.Collection(result, 
        //     expr => Assert.Equal(("1", '+', "2"), expr),
        //     expr => Assert.Equal(("4", '/', "3"), expr),
        //     expr => Assert.Equal(("5", '-', "{1}"), expr),
        //     expr => Assert.Equal(("{0}", '*', "{2}"), expr));
    }

    public static IEnumerable<object[]> DataForParse =>
    [
        ["(1 + 2 - 4 / (1 - 2)) - 2", new BinaryStrOperationContext[]
        {
            new("1", "-", "2"),
            new("4", "/", "{0}"),
            new("1", "+", "2"),
            new("{2}", "-", "{1}"),
            new("{3}", "-", "2")
        }],
        ["-23.56 + 2 * (-4.5 - (-8))", new BinaryStrOperationContext[]
        {
            new("-8"),
            new("-4.5", "-", "{0}"),
            new("2", "*", "{1}"),
            new("-23.56", "+", "{2}")
        }]
    ];



    [Theory]
    [MemberData(nameof(DataForOpenBrackets))]
    public void OpenBracketsTests(string expression, StrOperationContextBase[] expected)
    {
        // Arrange
        // var target = new StringParser(null);

        // Act
        var result = _target.OpenBrackets(expression);

        // Asserts
        Assert.True(result.Length == expected.Length);
        Assert.Collection(result, expected.Select(exp => new Action<StrOperationContextBase>(actual => Assert.Equal(exp, actual))).ToArray());
    }

    public static IEnumerable<object[]> DataForOpenBrackets =>
    [
        ["-33.01", new BinaryStrOperationContext[] { new ("-33.01") }],
        ["(-33.01 - 0.23)", new BinaryStrOperationContext[] { new ("-33.01", "-", "0.23") }],
        ["(-1 + 2) - ( -5 ) * 2", new BinaryStrOperationContext[]
        {
            new ("-5"),
            new ("-1", "+", "2"),
            new ("{0}", "*", "2"),
            new ("{1}", "-", "{2}")
        }],
        ["(1 + 2 - 4 / (1 - 2)) - 2", new BinaryStrOperationContext[]
        {
            new("1", "-", "2"),
            new("4", "/", "{0}"),
            new("1", "+", "2"),
            new("{2}", "-", "{1}"),
            new("{3}", "-", "2")
        }],
        ["-23.56 + 2 * (-4.5 - (-8))", new BinaryStrOperationContext[]
        {
            new("-8"),
            new("-4.5", "-", "{0}"),
            new("2", "*", "{1}"),
            new("-23.56", "+", "{2}")
        }]
    ];




    [Theory]
    [MemberData(nameof(DataForSplitExp))]
    public void SeparateExpressionTests(string expression, StrOperationContextBase[] expected)
    {
        // Arrange
        // var target = new StringParser();

        // Act
        // var result = target.SeparateExpression(expression, 0);
        var result = _target.SeparateExpression(expression, 0);

        // Asserts
        Assert.True(result.Length == expected.Length);
        // Assert.Collection(result, expected.Select(exp => new Action<string>(actual => Assert.Equal(exp, actual))).ToArray());
        Assert.Collection(result, expected.Select(exp => new Action<StrOperationContextBase>(actual => Assert.Equal(exp, actual))).ToArray());
    }

    public static IEnumerable<object[]> DataForSplitExp =>
    [
        // ["1 * 2 - 4 / 3 + 2", new[] { "1 * 2", "4 / 3", "{0} - {1}", "{2} + 2" }],
        // ["1 + 2 * 1 + 2", new[] { "2 * 1", "1 + {0}", "{1} + 2" }],
        // ["1 * 2 + 1 * 2.3", new[] { "1 * 2", "1 * 2.3 + {0} + {1}" }],
        // ["1 * 2 + 1 * 2", new[] { "1 * 2", "{0} + {0}" }]
        // ["2 * 3 + 6.4 / 2 * 3.89", new BinaryStrOperationContext[]
        // {
        // new ("2", '*', "3"),
        // new ("6.4", '/', "2"),
        // new ("{1}", '*', "3.89"),
        // new ("{0}", '+', "{2}"),
        // }],
        // ["-2 * 3 + (-6) / 2 * 3", new BinaryStrOperationContext[] // здесь (-6) не будет. скобки будут раскрываться.
        // ["2 * 3 + 6 / 2 * 3", new BinaryStrOperationContext[]
        // ["2 * 3 ! + 6 / 2 * 3", new BinaryStrOperationContext[]
        ["-12.21", new BinaryStrOperationContext[]
        {
            new ("-12.21", "+", "0")
        }],
        ["6!", new UnaryStrOperationContext[]
        {
            new ("6", "!")
        }],
        ["!6", new UnaryStrOperationContext[]
        {
            new ("6", "!")
        }],
        ["-6 / 3", new BinaryStrOperationContext[]
        {
            new ("-6", "/", "3")
        }],
        ["6! + 9", new StrOperationContextBase[]
        {
            new UnaryStrOperationContext("6", "!"),
            new BinaryStrOperationContext("{0}", "+", "9")
        }],        
        ["!6 + 9", new StrOperationContextBase[]
        {
            new UnaryStrOperationContext ("6", "!"),
            new BinaryStrOperationContext("{0}", "+", "9")
        }],
        ["-9 + 6!", new StrOperationContextBase[]
        {
            new UnaryStrOperationContext("6", "!"),
            new BinaryStrOperationContext("-9", "+", "{0}")
        }],
        ["-2 * 3 + 6 / 3 + 6", new BinaryStrOperationContext[]
        {
            new ("-2", "*", "3"),
            new ("6", "/", "3"),
            new ("{0}", "+", "{1}"),
            new ("{2}", "+", "6")
        }]
    ];






    // [Theory]
    // [MemberData(nameof(DataBrackets2))]
    // public void OpenBrackets2(string expression, string[] expected)
    // {
    //     // Arrange
    //     var target = new StringParser(null);
    //
    //     // Act
    //     string[] result = target.OpenBrackets2(expression);
    //
    //     // Asserts
    //     Assert.True(result.Length == expected.Length);
    //     Assert.Collection(result, expected.Select(exp => new Action<string>(actual => Assert.Equal(exp, actual))).ToArray());
    //     // Assert.Collection(result,
    //     //     expr => Assert.Equal("4 / 3", expr),
    //     //     expr => Assert.Equal("1 + 2", expr),
    //     //     expr => Assert.Equal("{1} - {1}", expr),
    //     //     expr => Assert.Equal("{2} + {0}", expr));
    // }

    public static IEnumerable<object[]> DataBrackets2 =>
    [
        ["(1 + 2 - 4 / (1 - 2)) - 2", new[] { "1 - 2", "1 + 2 - 4 / {0}", "{1} - 2" }],
        ["((1 + 2) - (1 + 2)) + (4 / 3)", new[] { "4 / 3", "1 + 2", "{1} - {1}", "{2} + {0}" }],
        ["(-1 + 2) - ( -5 ) + 2", new[] { " -5 ", "-1 + 2", "{1} - {0} + 2" }]
    ];

    // (1 + 2 - (6 - 2 * 4)) + (5 - (4 / 3))    {0}: 4 / 3
    // (1 + 2 - (6 - 2 * 4)) + (5 - {0})        {1}: 5 - {0}
    // (1 + 2 - (6 - 2 * 4)) + {1}              {2}: 6 - 2 * 4
    // (1 + 2 - {2}) + {1}                      {3}: 1 + 2 - {2}
    // {3} + {1}                                {4}: {3} + {1}


    // ((1 + 2) - (1 + 2)) + (4 / 3)
    // ((1 + 2) - (1 + 2)) + {0}                {0}: 4 / 3
    // ({1} - {1}) + {0}                        {1}: 1 + 2
    // {2} + {0}                                {2}: {1} - {1}
    //                                          {3}: {2} + {0}

}
