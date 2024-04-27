using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Ast.Syntax.Statements;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Tests.Infrastructure;
using Baila.CSharp.Typing;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using Xunit.Abstractions;

namespace Baila.CSharp.Tests;

public class ForStatementTests : TestsBase
{
    public ForStatementTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Theory]
    [InlineData("""
                for i = 1 to 3 {
                   body();
                }
                """)]
    [InlineData("""
                for i=1 to 3{
                   body();
                }
                """)]
    [InlineData("""
                for i = 1 to 3 {
                   body(); }
                """)]
    [InlineData("for i = 1 to 3 { body(); }")]
    [InlineData("""
                for i = 1 to 3
                { body(); }
                """)]
    [InlineData("""
                for i = 1 to 3
                {
                    body(); }
                """)]
    [InlineData("""
                for i = 1 to 3
                {
                    body();
                }
                """)]
    public void TestParsing(string source)
    {
        var program = CompileProgram(source);

        AssertAst(
            program,
            a =>
            {
                a.AssertRootStmt<ForStatement>(
                    stmt =>
                    {
                        stmt.CounterVariable.Should().Be("i");
                        a.AssertInner<IntValueExpression>(
                            stmt.InitialValue,
                            ive => ive.Value == 1);
                        a.AssertInner<IntValueExpression>(
                            stmt.FinalValue,
                            ive => ive.Value == 3);
                        a.AssertInnerBlock(
                            stmt.Body,
                            (_, innerAsserter) =>
                            {
                                innerAsserter.AssertRootExprStmt<FunctionCallExpression>(
                                    fce =>
                                    {
                                        innerAsserter.AssertInner<VariableExpression>(
                                            fce.FunctionHolder,
                                            ve => ve.Name == "body");
                                    });
                            });
                    });
            });
    }

    [Fact]
    public void TestCounterScopes()
    {
        var program = CompileProgram("""
                                     for i = 1 to 3 {}
                                     """);

        Assert.False(NameTable.Exists("i"), "Loop counter should not exist at this point in time");
        program.Execute();
        Assert.False(NameTable.Exists("i"), "Loop counter should not be exposed to the scope after loop ran");
    }

    [Fact]
    public void TestUpperBoundInclusivity()
    {
        var program = CompileProgram("""
                                     for i = 1 to 5 { print(i) }
                                     """);

        var counts = 0;

        var callback = Substitute.For<IBailaCallable>();
        callback.Call(Arg.Any<BailaCallableArgs>()).ReturnsNull().AndDoes(_ => { counts++; });

        var function = new FunctionValue();
        function.AddOverload(
            new FunctionOverload(
                callback,
                [
                    new FunctionParameter("x", BailaType.Any)
                ],
                null
            )
        );
        NameTable.AddConstant("print", function);

        program.Execute();

        counts.Should().Be(5);
    }
}