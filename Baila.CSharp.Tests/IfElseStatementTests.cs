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

public class IfElseStatementTests : TestsBase
{
    public IfElseStatementTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Theory]
    [InlineData("""
                if a == 3 && b == 7 {
                   body();
                }
                """)]
    [InlineData("""
                if a==3&&b==7{
                   body();
                }
                """)]
    [InlineData("""
                if a == 3 && b == 7 {
                   body(); }
                """)]
    [InlineData("if a == 3 && b == 7 { body(); }")]
    [InlineData("""
                if a == 3 && b == 7
                { body(); }
                """)]
    [InlineData("""
                if a == 3 && b == 7
                {
                    body(); }
                """)]
    [InlineData("""
                if a == 3 && b == 7
                {
                    body();
                }
                """)]
    public void ThenBranchOnly_Block_TestParsing(string source)
    {
        var program = CompileProgram(source);

        AssertAst(
            program,
            a =>
            {
                a.AssertRootStmt<IfElseStatement>(
                    stmt =>
                    {
                        a.AssertInnerFunc<BinaryExpression>(
                            stmt.Condition,
                            andBin =>
                                andBin is
                                {
                                    Left: BinaryExpression
                                    {
                                        Left: VariableExpression { Name: "a" },
                                        Right: IntValueExpression { Value: 3 }
                                    },
                                    Right: BinaryExpression
                                    {
                                        Left: VariableExpression { Name: "b" },
                                        Right: IntValueExpression { Value: 7 }
                                    }
                                });
                        a.AssertInnerBlock(
                            stmt.TrueStatement,
                            (_, a2) =>
                            {
                                a2.AssertRootExprStmt<FunctionCallExpression>(
                                    fce =>
                                    {
                                        a2.AssertInner<VariableExpression>(
                                            fce.FunctionHolder,
                                            ve => ve.Name == "body");
                                    });
                            });
                        stmt.FalseStatement.Should().BeNull();
                    });
            });
    }

    [Theory]
    [InlineData("""
                if a == 3 && b == 7 body();
                """)]
    [InlineData("""
                if a == 3 && b == 7
                    body();
                """)]
    [InlineData("""
                if a == 3 && b == 7
                
                    body();
                """)]
    [InlineData("""
                if a == 3 && b == 7
                
                
                    body();
                """)]
    public void ThenBranchOnly_OneStatement_TestParsing(string source)
    {
        var program = CompileProgram(source);

        AssertAst(
            program,
            a =>
            {
                a.AssertRootStmt<IfElseStatement>(
                    stmt =>
                    {
                        a.AssertInnerFunc<BinaryExpression>(
                            stmt.Condition,
                            andBin =>
                                andBin is
                                {
                                    Left: BinaryExpression
                                    {
                                        Left: VariableExpression { Name: "a" },
                                        Right: IntValueExpression { Value: 3 }
                                    },
                                    Right: BinaryExpression
                                    {
                                        Left: VariableExpression { Name: "b" },
                                        Right: IntValueExpression { Value: 7 }
                                    }
                                });
                        a.AssertInnerFunc<FunctionCallExpression>(
                            stmt.TrueStatement,
                            fce => fce.FunctionHolder is VariableExpression { Name: "body" });
                        stmt.FalseStatement.Should().BeNull();
                    });
            });
    }

    [Theory]
    [InlineData("""
                if a == 3 && b == 7 if c == 5 body();
                """)]
    [InlineData("""
                if a == 3 && b == 7 if c == 5
                    body();
                """)]
    [InlineData("""
                if a == 3 && b == 7 if c == 5
                
                    body();
                """)]
    [InlineData("""
                if a == 3 && b == 7 if c == 5
                
                
                    body();
                """)]
    [InlineData("""
                if a == 3 && b == 7
                    if c == 5 body();
                """)]
    [InlineData("""
                if a == 3 && b == 7
                    if c == 5
                        body();
                """)]
    [InlineData("""
                if a == 3 && b == 7
                    if c == 5
                
                        body();
                """)]
    [InlineData("""
                if a == 3 && b == 7
                    if c == 5
                
                
                        body();
                """)]
    [InlineData("""
                if a == 3 && b == 7
                
                    if c == 5
                
                        body();
                """)]
    [InlineData("""
                if a == 3 && b == 7
                
                    if c == 5
                
                
                        body();
                """)]
    [InlineData("""
                if a == 3 && b == 7
                
                
                    if c == 5
                
                        body();
                """)]
    [InlineData("""
                if a == 3 && b == 7
                
                
                    if c == 5
                
                
                        body();
                """)]
    public void ThenBranchOnly_InnerIf_TestParsing(string source)
    {
        var program = CompileProgram(source);

        AssertAst(
            program,
            a =>
            {
                a.AssertRootStmt<IfElseStatement>(
                    stmt =>
                    {
                        a.AssertInnerStmt<IfElseStatement>(
                            stmt.TrueStatement,
                            assertFunc: ies => IsStmtFunctionCall(ies.TrueStatement, "body")
                        );
                        stmt.FalseStatement.Should().BeNull();
                    });
            });
    }

    [Theory]
    [InlineData("""
                if a == 3 && b == 7 {
                   body();
                } else { body2(); }
                """)]
    [InlineData("""
                if a==3&&b==7{
                   body();
                } else { body2(); }
                """)]
    [InlineData("""
                if a == 3 && b == 7 {
                   body(); } else { body2(); }
                """)]
    [InlineData("if a == 3 && b == 7 { body(); } else { body2(); }")]
    [InlineData("""
                if a == 3 && b == 7
                { body(); } else { body2(); }
                """)]
    [InlineData("""
                if a == 3 && b == 7
                {
                    body(); } else { body2(); }
                """)]
    [InlineData("""
                if a == 3 && b == 7
                {
                    body();
                } else { body2(); }
                """)]
    [InlineData("""
                if a == 3 && b == 7 {
                   body();
                } else {
                    body2();
                }
                """)]
    [InlineData("""
                if a==3&&b==7{
                   body();
                } else {
                    body2();
                }
                """)]
    [InlineData("""
                if a == 3 && b == 7 {
                   body(); } else {
                    body2();
                }
                """)]
    [InlineData("""
                if a == 3 && b == 7
                { body(); } else {
                    body2();
                }
                """)]
    [InlineData("""
                if a == 3 && b == 7
                {
                    body(); } else {
                        body2();
                    }
                """)]
    [InlineData("""
                if a == 3 && b == 7
                {
                    body();
                } else {
                    body2();
                }
                """)]
    [InlineData("""
                if a == 3 && b == 7 {
                   body();
                }
                else {
                    body2();
                }
                """)]
    [InlineData("""
                if a==3&&b==7{
                   body();
                }
                else {
                    body2();
                }
                """)]
    [InlineData("""
                if a == 3 && b == 7 {
                   body(); }
                else {
                    body2();
                }
                """)]
    [InlineData("""
                if a == 3 && b == 7
                { body(); }
                else {
                    body2();
                }
                """)]
    [InlineData("""
                if a == 3 && b == 7
                {
                    body(); }
                else {
                    body2();
                }
                """)]
    [InlineData("""
                if a == 3 && b == 7
                {
                    body();
                }
                else {
                    body2();
                }
                """)]
    [InlineData("""
                if a == 3 && b == 7 {
                   body();
                }
                else
                {
                    body2();
                }
                """)]
    [InlineData("""
                if a==3&&b==7{
                   body();
                }
                else
                {
                    body2();
                }
                """)]
    [InlineData("""
                if a == 3 && b == 7 {
                   body(); }
                else
                {
                    body2();
                }
                """)]
    [InlineData("""
                if a == 3 && b == 7
                { body(); }
                else
                {
                    body2();
                }
                """)]
    [InlineData("""
                if a == 3 && b == 7
                {
                    body(); }
                else
                {
                    body2();
                }
                """)]
    [InlineData("""
                if a == 3 && b == 7
                {
                    body();
                }
                else
                {
                    body2();
                }
                """)]
    public void ThenBranchAndElseBranch_Block_TestParsing(string source)
    {
        var program = CompileProgram(source);

        AssertAst(
            program,
            a =>
            {
                a.AssertRootStmt<IfElseStatement>(
                    stmt =>
                    {
                        a.AssertInnerFunc<BinaryExpression>(
                            stmt.Condition,
                            andBin =>
                                andBin is
                                {
                                    Left: BinaryExpression
                                    {
                                        Left: VariableExpression { Name: "a" },
                                        Right: IntValueExpression { Value: 3 }
                                    },
                                    Right: BinaryExpression
                                    {
                                        Left: VariableExpression { Name: "b" },
                                        Right: IntValueExpression { Value: 7 }
                                    }
                                });
                        a.AssertInnerBlock(
                            stmt.TrueStatement,
                            (_, a2) =>
                            {
                                a2.AssertRootExprStmt<FunctionCallExpression>(
                                    fce =>
                                    {
                                        a2.AssertInner<VariableExpression>(
                                            fce.FunctionHolder,
                                            ve => ve.Name == "body");
                                    });
                            });
                        stmt.FalseStatement.Should().NotBeNull();
                        a.AssertInnerBlock(
                            stmt.FalseStatement!,
                            (_, a2) =>
                            {
                                a2.AssertRootExprStmt<FunctionCallExpression>(
                                    fce =>
                                    {
                                        a2.AssertInner<VariableExpression>(
                                            fce.FunctionHolder,
                                            ve => ve.Name == "body2");
                                    });
                            });
                    });
            });
    }

    [Theory]
    [InlineData("if 3 == 3 { trueFunction() } else { falseFunction() }", "trueFunction")]
    [InlineData("if 3 != 3 { trueFunction() } else { falseFunction() }", "falseFunction")]
    public void EvaluationTest(string source, string expectedCalledFunctionName)
    {
        var program = CompileProgram(source);
        string? actualCalledFunctionName = null;

        var trueFunction = Substitute.For<IBailaCallable>();
        trueFunction.Call(Arg.Any<BailaCallableArgs>()).ReturnsNull().AndDoes(_ => actualCalledFunctionName = "trueFunction");
        NameTable.AddConstant("trueFunction", FunctionValue.WithOverload(new FunctionOverload(trueFunction, [], BailaType.String)));

        var falseFunction = Substitute.For<IBailaCallable>();
        falseFunction.Call(Arg.Any<BailaCallableArgs>()).ReturnsNull().AndDoes(_ => actualCalledFunctionName = "falseFunction");
        NameTable.AddConstant("falseFunction", FunctionValue.WithOverload(new FunctionOverload(falseFunction, [], BailaType.String)));
        
        program.Execute();

        actualCalledFunctionName.Should().Be(expectedCalledFunctionName);
    }
}