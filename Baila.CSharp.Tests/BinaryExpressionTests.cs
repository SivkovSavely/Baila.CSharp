using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Tests.Infrastructure;
using Baila.CSharp.Typing;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit.Abstractions;

namespace Baila.CSharp.Tests;

public class BinaryExpressionTests : TestsBase
{
    public BinaryExpressionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Theory]
    [InlineData("1 + 1", "2", nameof(BailaType.Int))]
    [InlineData("1 + 1 + 1", "3", nameof(BailaType.Int))]
    [InlineData("1.1 + 1.1", "2.2", nameof(BailaType.Float))]
    [InlineData("1.1 + 1.1 + 2.2", "4.4", nameof(BailaType.Float))]
    public void PlusTest(string source, string expectedValueStringified, string expectedResultType)
    {
        var program = CompileProgram(source);

        program.Execute();

        var value = program.LastEvaluatedValue;
        value.Should().NotBeNull();
        value!.GetBailaType().Should().Be(GetBailaTypeByName(expectedResultType));
        value.GetAsString().Should().Be(expectedValueStringified);
    }

    [Theory]
    [MemberData(nameof(GetAllBinaryOperators))]
    public void TestAllBinaryOperatorsHaveCallbacks(BinaryExpression.Operator binaryOperator)
    {
        Assert.Contains(binaryOperator, BinaryExpression.BinaryOperators);
    }

    [Theory]
    [MemberData(nameof(GetAllBinaryOperatorCallbacks))]
    public void TestAllBinaryOperatorsCallbacksHaveOperators(BinaryExpression.Operator binaryOperator)
    {
        Assert.Contains(binaryOperator, BinaryExpression.Operator.All);
    }

    public static IEnumerable<object[]> GetAllBinaryOperators()
    {
        return BinaryExpression.Operator.All.Select(x => new[] { x });
    }

    public static IEnumerable<object[]> GetAllBinaryOperatorCallbacks()
    {
        return BinaryExpression.BinaryOperators.Keys.Select(x => new[] { x });
    }
}