using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Types;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Tests.Infrastructure;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit.Abstractions;

namespace Baila.CSharp.Tests;

public class UnaryExpressionTests : TestsBase
{
    public UnaryExpressionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Theory]
    [InlineData("+123", "123", nameof(BailaType.Int))]
    [InlineData("+123.5", "123.5", nameof(BailaType.Float))]
    [InlineData("-123", "-123", nameof(BailaType.Int))]
    [InlineData("-123.5", "-123.5", nameof(BailaType.Float))]
    [InlineData("!true", "false", nameof(BailaType.Bool))]
    [InlineData("!false", "true", nameof(BailaType.Bool))]
    [InlineData("~1", "-2", nameof(BailaType.Int))]
    public void CorrectEvaluationTest(string source, string expectedValueStringified, string expectedResultType)
    {
        var program = CompileProgram(source.TrimEnd(' '));

        program.Execute();

        var value = program.LastEvaluatedValue;
        value.Should().NotBeNull();
        value!.GetBailaType().Should().Be(GetBailaTypeByName(expectedResultType));
        value.GetAsString().Should().Be(expectedValueStringified.TrimEnd(' '));
    }

    [Theory]
    [MemberData(nameof(UnaryOperators))]
    public void UnaryOperatorsHaveCallbacks(PrefixUnaryExpression.Operator unaryOperator)
    {
        Assert.Contains(unaryOperator, PrefixUnaryExpression.UnaryOperators);
    }

    [Theory]
    [MemberData(nameof(UnaryOperatorCallbacks))]
    public void UnaryOperatorsCallbacksHaveOperators(PrefixUnaryExpression.Operator unaryOperator)
    {
        Assert.Contains(unaryOperator, PrefixUnaryExpression.Operator.All);
    }

    public static IEnumerable<object[]> UnaryOperators()
    {
        return PrefixUnaryExpression.Operator.All.Select(x => new[] { x });
    }

    public static IEnumerable<object[]> UnaryOperatorCallbacks()
    {
        return PrefixUnaryExpression.UnaryOperators.Keys.Select(x => new[] { x });
    }
}