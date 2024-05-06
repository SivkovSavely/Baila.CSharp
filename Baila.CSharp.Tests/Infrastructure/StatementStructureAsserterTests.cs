using Baila.CSharp.Ast.Lexer;
using Baila.CSharp.Ast.Syntax;
using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Ast.Syntax.Statements;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Baila.CSharp.Tests.Infrastructure;

#pragma warning disable xUnit1000
internal class StatementStructureAsserterTests : TestsBase
#pragma warning restore xUnit1000
{
    public const string Filename = "test.baila";

    public StatementStructureAsserterTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public void CorrectAssertion_AssertsCorrectly()
    {
        var ast = new Statements();
        ast.AddStatement(
            new IfElseStatement(
                null!,
                new IntValueExpression(1, new Token("", SyntaxNodeSpan.Empty, TokenType.NumberLiteral, "0")),
                new ExpressionStatement(
                    new FunctionCallExpression(
                        new VariableExpression("hello", null!),
                        null!,
                        [],
                        null!
                    )
                ),
                null
            )
        );

        AssertAst(
            ast,
            a => { a.AssertRootStmt<IfElseStatement>(); });
    }

    [Fact]
    public void WrongAssertion_TestFails()
    {
        var ast = new Statements();
        ast.AddStatement(
            new IfElseStatement(
                null!,
                new IntValueExpression(1, new Token("", SyntaxNodeSpan.Empty, TokenType.NumberLiteral, "0")),
                new ExpressionStatement(
                    new FunctionCallExpression(
                        new VariableExpression("hello",
                            new Token("", SyntaxNodeSpan.Empty, TokenType.NumberLiteral, "0")),
                        null!,
                        [],
                        null!
                    )
                ),
                null
            )
        );

        Assert.ThrowsAny<XunitException>(() =>
        {
            AssertAst(
                ast,
                a => { });
        });
    }

    [Fact]
    public void WithAction_CorrectAssertion_AssertsCorrectly()
    {
        var ast = new Statements();
        ast.AddStatement(
            new IfElseStatement(
                null!,
                new IntValueExpression(1, new Token("", SyntaxNodeSpan.Empty, TokenType.NumberLiteral, "0")),
                new ExpressionStatement(
                    new FunctionCallExpression(
                        new VariableExpression("hello",
                            new Token("", SyntaxNodeSpan.Empty, TokenType.NumberLiteral, "0")),
                        null!,
                        [],
                        null!
                    )
                ),
                null
            )
        );

        AssertAst(
            ast,
            a =>
            {
                a.AssertRootStmt<IfElseStatement>(
                    stmt =>
                    {
                        a.AssertInner<IntValueExpression>(
                            stmt.Condition,
                            ive => ive.Evaluate().GetAsInteger() == 1);
                        a.AssertInner<FunctionCallExpression>(
                            stmt.TrueStatement);
                        Assert.Null(stmt.FalseStatement);
                    });
            });
    }

    [Fact]
    public void WithAction_FalseAssertion_TestFails()
    {
        var ast = new Statements();
        ast.AddStatement(
            new IfElseStatement(
                null!,
                new IntValueExpression(1, new Token("", SyntaxNodeSpan.Empty, TokenType.NumberLiteral, "0")),
                new ExpressionStatement(
                    new FunctionCallExpression(
                        new VariableExpression("hello",
                            new Token("", SyntaxNodeSpan.Empty, TokenType.NumberLiteral, "0")),
                        null!,
                        [],
                        null!
                    )
                ),
                null
            )
        );

        Assert.ThrowsAny<XunitException>(() =>
        {
            AssertAst(
                ast,
                a =>
                {
                    a.AssertRootStmt<IfElseStatement>(
                        stmt =>
                        {
                            a.AssertInner<IntValueExpression>(
                                stmt.Condition,
                                ive => ive.Evaluate().GetAsInteger() == 1);
                            a.AssertInner<FunctionCallExpression>(
                                stmt.TrueStatement);
                            Assert.NotNull(stmt.FalseStatement);
                        });
                });
        });

        Assert.ThrowsAny<XunitException>(() =>
        {
            AssertAst(
                ast,
                a =>
                {
                    a.AssertRootStmt<IfElseStatement>(
                        stmt =>
                        {
                            a.AssertInner<IntValueExpression>(
                                stmt.Condition,
                                ive => ive.Evaluate().GetAsInteger() != 1);
                            a.AssertInner<FunctionCallExpression>(
                                stmt.TrueStatement);
                            Assert.Null(stmt.FalseStatement);
                        });
                });
        });

        Assert.ThrowsAny<XunitException>(() =>
        {
            AssertAst(
                ast,
                a =>
                {
                    a.AssertRootStmt<IfElseStatement>(
                        stmt =>
                        {
                            a.AssertInner<IntValueExpression>(
                                stmt.Condition,
                                ive => ive.Evaluate().GetAsInteger() == 1);
                            a.AssertInner<BinaryExpression>(
                                stmt.TrueStatement);
                            Assert.Null(stmt.FalseStatement);
                        });
                });
        });
    }

    [Fact]
    public void SeveralStatements_CorrectAssertion_AssertsCorrectly()
    {
        var ast = new Statements();
        ast.AddStatement(new IfElseStatement(new Token("", SyntaxNodeSpan.Empty, TokenType.If), null!, null!, null!));
        ast.AddStatement(new ReturnStatement(new Token("", SyntaxNodeSpan.Empty, TokenType.Return), null!));
        ast.AddStatement(new WhileStatement(new Token("", SyntaxNodeSpan.Empty, TokenType.While), null!, null!));

        AssertAst(
            ast,
            a =>
            {
                a.AssertRootStmt<IfElseStatement>(_ => { });
                a.AssertRootStmt<ReturnStatement>(_ => { });
                a.AssertRootStmt<WhileStatement>(_ => { });
            });
    }

    [Fact]
    public void SeveralStatements_SomeAreNotAsserted_TestFails()
    {
        var ast = new Statements();
        ast.AddStatement(new IfElseStatement(new Token("", SyntaxNodeSpan.Empty, TokenType.If), null!, null!, null!));
        ast.AddStatement(new ReturnStatement(new Token("", SyntaxNodeSpan.Empty, TokenType.Return), null!));
        ast.AddStatement(new WhileStatement(new Token("", SyntaxNodeSpan.Empty, TokenType.While), null!, null!));

        var exception = Assert.ThrowsAny<XunitException>(() =>
        {
            AssertAst(
                ast,
                a =>
                {
                    a.AssertRootStmt<IfElseStatement>(_ => { });
                    a.AssertRootStmt<ReturnStatement>(_ => { });
                });
        });
        Assert.Equal("There are 1 more unasserted statements:", exception.Message.Split("\n").First());
    }

    [Fact]
    public void SeveralStatements_ExtraAreAsserted_TestFails()
    {
        var ast = new Statements();
        ast.AddStatement(new IfElseStatement(new Token("", SyntaxNodeSpan.Empty, TokenType.If), null!, null!, null!));
        ast.AddStatement(new ReturnStatement(new Token("", SyntaxNodeSpan.Empty, TokenType.Return), null!));
        ast.AddStatement(new WhileStatement(new Token("", SyntaxNodeSpan.Empty, TokenType.While), null!, null!));

        var exception = Assert.ThrowsAny<XunitException>(() =>
        {
            AssertAst(
                ast,
                a =>
                {
                    a.AssertRootStmt<IfElseStatement>(_ => { });
                    a.AssertRootStmt<ReturnStatement>(_ => { });
                    a.AssertRootStmt<WhileStatement>(_ => { });
                    a.AssertRootStmt<ReturnStatement>(_ => { });
                });
        });
        Assert.Equal("Extra statement was asserted: ReturnStatement", exception.Message);
    }

    [Fact]
    public void AssertInnerBlock_CorrectAssertion_AssertsCorrectly()
    {
        var ast = new Statements();
        ast.AddStatement(
            new IfElseStatement(
                null!,
                new IntValueExpression(1, new Token("", SyntaxNodeSpan.Empty, TokenType.NumberLiteral, "0")),
                new BlockStatement(
                    null!,
                    new List<IStatement>
                    {
                        new ExpressionStatement(
                            new FunctionCallExpression(
                                new VariableExpression("hello",
                                    new Token("", SyntaxNodeSpan.Empty, TokenType.NumberLiteral, "0")),
                                null!,
                                [],
                                null!
                            )
                        ),
                        new ReturnStatement(new Token("", SyntaxNodeSpan.Empty, TokenType.Return), null!),
                        new WhileStatement(new Token("", SyntaxNodeSpan.Empty, TokenType.While), null!, null!)
                    },
                    null!
                ),
                null
            )
        );

        AssertAst(
            ast,
            a =>
            {
                a.AssertRootStmt<IfElseStatement>(
                    stmt =>
                    {
                        a.AssertInner<IntValueExpression>(
                            stmt.Condition,
                            ive => ive.Evaluate().GetAsInteger() == 1);
                        a.AssertInnerBlock(
                            stmt.TrueStatement,
                            (bs, innerAsserter) =>
                            {
                                innerAsserter.AssertRootExprStmt<FunctionCallExpression>();
                                innerAsserter.AssertRootStmt<ReturnStatement>();
                                innerAsserter.AssertRootStmt<WhileStatement>();
                            });
                        Assert.Null(stmt.FalseStatement);
                    });
            });
    }
}