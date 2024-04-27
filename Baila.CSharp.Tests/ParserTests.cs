using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Ast.Syntax.Statements;
using Baila.CSharp.Tests.Infrastructure;
using FluentAssertions;
using Xunit.Abstractions;

namespace Baila.CSharp.Tests;

public class ParserTests : TestsBase
{
    public ParserTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public void StringInterpolation_OneInterpolation_NoEnclosingFixedStrings()
    {
        var ast = CompileProgram("""
                                 "${123}"
                                 """);

        AssertAst(
            ast,
            a =>
            {
                a.AssertRootExprStmt<StringConcatExpression>(
                    expression =>
                    {
                        expression.FixedStrings.Should().Equal("");
                        expression.Expressions.Should().Equal(new IntValueExpression(123));
                    });
            });
        
        ast.Execute();
        ast.LastEvaluatedValue?.GetAsString().Should().Be("123");
    }

    [Fact]
    public void StringInterpolation_OneInterpolation_LeftEnclosingFixedString()
    {
        var ast = CompileProgram("""
                                 "Abc ${123}"
                                 """);

        AssertAst(
            ast,
            a =>
            {
                a.AssertRootExprStmt<StringConcatExpression>(
                    expression =>
                    {
                        expression.FixedStrings.Should().Equal("Abc ");
                        expression.Expressions.Should().Equal(new IntValueExpression(123));
                    });
            });
        
        ast.Execute();
        ast.LastEvaluatedValue?.GetAsString().Should().Be("Abc 123");
    }

    [Fact]
    public void StringInterpolation_OneInterpolation_RightEnclosingFixedString()
    {
        var ast = CompileProgram("""
                                 "${123} Def"
                                 """);

        AssertAst(
            ast,
            a =>
            {
                a.AssertRootExprStmt<StringConcatExpression>(
                    expression =>
                    {
                        expression.FixedStrings.Should().Equal("", " Def");
                        expression.Expressions.Should().Equal(new IntValueExpression(123));
                    });
            });
        
        ast.Execute();
        ast.LastEvaluatedValue?.GetAsString().Should().Be("123 Def");
    }

    [Fact]
    public void StringInterpolation_OneInterpolation_BothEnclosingFixedStrings()
    {
        var ast = CompileProgram("""
                                 "Abc ${123} Def"
                                 """);

        AssertAst(
            ast,
            a =>
            {
                a.AssertRootExprStmt<StringConcatExpression>(
                    expression =>
                    {
                        expression.FixedStrings.Should().Equal("Abc ", " Def");
                        expression.Expressions.Should().Equal(new IntValueExpression(123));
                    });
            });
        
        ast.Execute();
        ast.LastEvaluatedValue?.GetAsString().Should().Be("Abc 123 Def");
    }

    [Fact]
    public void StringInterpolation_TwoInterpolations_BothEnclosingFixedStrings_NoFixedStringsBetween()
    {
        var ast = CompileProgram("""
                                 "Abc ${123}${456} Def"
                                 """);

        AssertAst(
            ast,
            a =>
            {
                a.AssertRootExprStmt<StringConcatExpression>(
                    expression =>
                    {
                        expression.FixedStrings.Should().Equal("Abc ", "", " Def");
                        expression.Expressions.Should().Equal(new IntValueExpression(123), new IntValueExpression(456));
                    });
            });
        
        ast.Execute();
        ast.LastEvaluatedValue?.GetAsString().Should().Be("Abc 123456 Def");
    }

    [Fact]
    public void StringInterpolation_TwoInterpolations_BothEnclosingFixedStrings_FixedStringBetween()
    {
        var ast = CompileProgram("""
                                 "Abc ${123} Ghi ${456} Def"
                                 """);

        AssertAst(
            ast,
            a =>
            {
                a.AssertRootExprStmt<StringConcatExpression>(
                    expression =>
                    {
                        expression.FixedStrings.Should().Equal("Abc ", " Ghi ", " Def");
                        expression.Expressions.Should().Equal(new IntValueExpression(123), new IntValueExpression(456));
                    });
            });
        
        ast.Execute();
        ast.LastEvaluatedValue?.GetAsString().Should().Be("Abc 123 Ghi 456 Def");
    }

    [Fact]
    public void StringInterpolation_SimpleVariableInterpolation()
    {
        var ast = CompileProgram("""
                                 var def = "def string"
                                 "Abc $def"
                                 """);

        AssertAst(
            ast,
            a =>
            {
                a.AssertRootStmt<VariableDefineStatement>();
                a.AssertRootExprStmt<StringConcatExpression>(
                    expression =>
                    {
                        expression.FixedStrings.Should().Equal("Abc ");
                        expression.Expressions.Should().Equal(new VariableExpression("def"));
                    });
            });
        
        ast.Execute();
        ast.LastEvaluatedValue?.GetAsString().Should().Be("Abc def string");
    }

    [Fact]
    public void StringInterpolation_VariableInInterpolation_EvaluatesArgumentsAtTheTimeOfEvaluation()
    {
        var ast = CompileProgram("""
                                 var s = "xyz"
                                 var s2 = "123${s}456"
                                 s = "abc"
                                 s2
                                 """);
        
        ast.Execute();
        ast.LastEvaluatedValue?.GetAsString().Should().Be("123xyz456");
    }
}