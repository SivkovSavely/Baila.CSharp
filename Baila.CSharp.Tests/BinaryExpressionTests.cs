using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Runtime.Types;
using Baila.CSharp.Tests.Infrastructure;
using FluentAssertions;
using Xunit.Abstractions;

namespace Baila.CSharp.Tests;

public class BinaryExpressionTests : TestsBase
{
    public BinaryExpressionTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Theory]
    // Addition
    [InlineData("1 + 1          ", "2  ", nameof(BailaType.Int))]
    [InlineData("1 + 1 + 1      ", "3  ", nameof(BailaType.Int))]
    [InlineData("1.1 + 1.1      ", "2.2", nameof(BailaType.Float))]
    [InlineData("1 + 1.1        ", "2.1", nameof(BailaType.Float))]
    [InlineData("1.1 + 1.1 + 2.2", "4.4", nameof(BailaType.Float))]
    // String concatenation
    [InlineData(""" "abc" + "def"         """, "abcdef   ", nameof(BailaType.String))]
    [InlineData(""" "abc" + "def" + "ghi" """, "abcdefghi", nameof(BailaType.String))]
    // Subtraction
    [InlineData("5 - 3          ", "2   ", nameof(BailaType.Int))]
    [InlineData("50 - 24 - 11   ", "15  ", nameof(BailaType.Int))]
    [InlineData("5.0 - 2.4 - 1.1", "1.5 ", nameof(BailaType.Float))]
    [InlineData("123 - 10.5     ", "112.5", nameof(BailaType.Float))]
    [InlineData("142.3 - 11     ", "131.3", nameof(BailaType.Float))]
    // Multiplication
    [InlineData("2 * 3      ", "6  ", nameof(BailaType.Int))]
    [InlineData("2 * 3 *   4", "24 ", nameof(BailaType.Int))]
    [InlineData("2.1 * 2    ", "4.2", nameof(BailaType.Float))]
    [InlineData("2.1 * 2 * 2", "8.4", nameof(BailaType.Float))]
    // String repetition
    [InlineData(""" "a" * 3   """, "aaa      ", nameof(BailaType.String))]
    [InlineData(""" "abc" * 3 """, "abcabcabc", nameof(BailaType.String))]
    [InlineData(""" 3 * "a"   """, "aaa      ", nameof(BailaType.String))]
    [InlineData(""" 3 * "abc" """, "abcabcabc", nameof(BailaType.String))]
    // Power
    [InlineData("123 ** 0 ", "1   ", nameof(BailaType.Int))]
    [InlineData("64 ** 0.5", "8   ", nameof(BailaType.Float))]
    [InlineData("2 ** 2   ", "4   ", nameof(BailaType.Int))]
    [InlineData("1.5 ** 2 ", "2.25", nameof(BailaType.Float))]
    // Equality and inequality
    [InlineData("1 == 1          ", "true ", nameof(BailaType.Bool))]
    [InlineData("1 != 1          ", "false", nameof(BailaType.Bool))]
    [InlineData("1 == 2          ", "false", nameof(BailaType.Bool))]
    [InlineData("1 != 2          ", "true ", nameof(BailaType.Bool))]
    [InlineData(""" "1" == "1" """, "true ", nameof(BailaType.Bool))]
    [InlineData(""" "1" != "1" """, "false", nameof(BailaType.Bool))]
    [InlineData(""" "1" == "2" """, "false", nameof(BailaType.Bool))]
    [InlineData(""" "1" != "2" """, "true ", nameof(BailaType.Bool))]
    [InlineData("1 == 1.0        ", "true ", nameof(BailaType.Bool))]
    [InlineData("1.0 == 1        ", "true ", nameof(BailaType.Bool))]
    [InlineData("1.0 == 1.0      ", "true ", nameof(BailaType.Bool))]
    [InlineData("1 != 1.0        ", "false", nameof(BailaType.Bool))]
    [InlineData("1.0 != 1        ", "false", nameof(BailaType.Bool))]
    [InlineData("1.0 != 1.0      ", "false", nameof(BailaType.Bool))]
    [InlineData("true == true    ", "true ", nameof(BailaType.Bool))]
    [InlineData("true == false   ", "false", nameof(BailaType.Bool))]
    [InlineData("true != true    ", "false", nameof(BailaType.Bool))]
    [InlineData("true != false   ", "true ", nameof(BailaType.Bool))]
    // Boolean logic
    [InlineData("true && true  ", "true ", nameof(BailaType.Bool))]
    [InlineData("true && false ", "false", nameof(BailaType.Bool))]
    [InlineData("false && false", "false", nameof(BailaType.Bool))]
    [InlineData("false && true ", "false", nameof(BailaType.Bool))]
    [InlineData("true || true  ", "true ", nameof(BailaType.Bool))]
    [InlineData("true || false ", "true ", nameof(BailaType.Bool))]
    [InlineData("false || false", "false", nameof(BailaType.Bool))]
    [InlineData("false || true ", "true ", nameof(BailaType.Bool))]
    // Numerical comparison
    [InlineData("1 < 1   ", "false", nameof(BailaType.Bool))]
    [InlineData("1 <= 1  ", "true ", nameof(BailaType.Bool))]
    [InlineData("1 < 2   ", "true ", nameof(BailaType.Bool))]
    [InlineData("1 <= 2  ", "true ", nameof(BailaType.Bool))]
    [InlineData("1 > 1   ", "false", nameof(BailaType.Bool))]
    [InlineData("1 >= 1  ", "true ", nameof(BailaType.Bool))]
    [InlineData("1 > 2   ", "false", nameof(BailaType.Bool))]
    [InlineData("1 >= 2  ", "false", nameof(BailaType.Bool))]
    [InlineData("1.0 < 1 ", "false", nameof(BailaType.Bool))]
    [InlineData("1.0 <= 1", "true ", nameof(BailaType.Bool))]
    [InlineData("1.0 < 2 ", "true ", nameof(BailaType.Bool))]
    [InlineData("1.0 <= 2", "true ", nameof(BailaType.Bool))]
    [InlineData("1.0 > 1 ", "false", nameof(BailaType.Bool))]
    [InlineData("1.0 >= 1", "true ", nameof(BailaType.Bool))]
    [InlineData("1.0 > 2 ", "false", nameof(BailaType.Bool))]
    [InlineData("1.0 >= 2", "false", nameof(BailaType.Bool))]
    [InlineData("1 < 1.0 ", "false", nameof(BailaType.Bool))]
    [InlineData("1 <= 1.0", "true ", nameof(BailaType.Bool))]
    [InlineData("1 < 2.0 ", "true ", nameof(BailaType.Bool))]
    [InlineData("1 <= 2.0", "true ", nameof(BailaType.Bool))]
    [InlineData("1 > 1.0 ", "false", nameof(BailaType.Bool))]
    [InlineData("1 >= 1.0", "true ", nameof(BailaType.Bool))]
    [InlineData("1 > 2.0 ", "false", nameof(BailaType.Bool))]
    [InlineData("1 >= 2.0", "false", nameof(BailaType.Bool))]
    // Common precedence tests
    [InlineData("2 + 2 * 2", "6", nameof(BailaType.Int))]
    [InlineData("2 * 3 ** 4", "162", nameof(BailaType.Int))]
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