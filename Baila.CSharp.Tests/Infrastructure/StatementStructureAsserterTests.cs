﻿using Baila.CSharp.Ast.Syntax;
using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Ast.Syntax.Statements;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Baila.CSharp.Tests.Infrastructure;

public class StatementStructureAsserterTests : TestsBase
{
    public const string Filename = "test.baila";
    public StatementStructureAsserterTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public void CorrectAssertion_AssertsCorrectly()
    {
        var ast = new Statements(Filename, SyntaxNodeSpan.Empty);
        ast.AddStatement(
            new IfElseStatement(
                new IntValueExpression(1, Filename, SyntaxNodeSpan.Empty),
                new ExpressionStatement(
                    new FunctionCallExpression(
                        new VariableExpression("hello", Filename, SyntaxNodeSpan.Empty),
                        [],
                        Filename,
                        SyntaxNodeSpan.Empty
                    ),
                    Filename,
                    SyntaxNodeSpan.Empty
                ),
                null,
                Filename,
                SyntaxNodeSpan.Empty
            )
        );

        AssertAst(
            ast,
            a => { a.AssertRootStmt<IfElseStatement>(); });
    }

    [Fact]
    public void WrongAssertion_TestFails()
    {
        var ast = new Statements(Filename, SyntaxNodeSpan.Empty);
        ast.AddStatement(
            new IfElseStatement(
                new IntValueExpression(1, Filename, SyntaxNodeSpan.Empty),
                new ExpressionStatement(
                    new FunctionCallExpression(
                        new VariableExpression("hello",
                            Filename,
                            SyntaxNodeSpan.Empty),
                        [],
                        Filename,
                        SyntaxNodeSpan.Empty
                    ),
                    Filename,
                    SyntaxNodeSpan.Empty
                ),
                null,
                Filename,
                SyntaxNodeSpan.Empty
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
        var ast = new Statements(Filename,
            SyntaxNodeSpan.Empty);
        ast.AddStatement(
            new IfElseStatement(
                new IntValueExpression(1,
                    Filename,
                    SyntaxNodeSpan.Empty),
                new ExpressionStatement(
                    new FunctionCallExpression(
                        new VariableExpression("hello",
                            Filename,
                            SyntaxNodeSpan.Empty),
                        [],
                        Filename,
                        SyntaxNodeSpan.Empty
                    ),
                    Filename,
                    SyntaxNodeSpan.Empty
                ),
                null,
                Filename,
                SyntaxNodeSpan.Empty
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
        var ast = new Statements(
            Filename,
            SyntaxNodeSpan.Empty);
        ast.AddStatement(
            new IfElseStatement(
                new IntValueExpression(1,
                    Filename,
                    SyntaxNodeSpan.Empty),
                new ExpressionStatement(
                    new FunctionCallExpression(
                        new VariableExpression("hello",
                            Filename,
                            SyntaxNodeSpan.Empty),
                        [],
                        Filename,
                        SyntaxNodeSpan.Empty
                    ),
                    Filename,
                    SyntaxNodeSpan.Empty
                ),
                null,
                Filename,
                SyntaxNodeSpan.Empty
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
        var ast = new Statements(Filename, SyntaxNodeSpan.Empty);
        ast.AddStatement(new IfElseStatement(null!, null!, null, Filename, SyntaxNodeSpan.Empty));
        ast.AddStatement(new ReturnStatement(null, Filename, SyntaxNodeSpan.Empty));
        ast.AddStatement(new WhileStatement(null!, null!, Filename, SyntaxNodeSpan.Empty));

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
        var ast = new Statements(Filename, SyntaxNodeSpan.Empty);
        ast.AddStatement(new IfElseStatement(null!, null!, null, Filename, SyntaxNodeSpan.Empty));
        ast.AddStatement(new ReturnStatement(null, Filename, SyntaxNodeSpan.Empty));
        ast.AddStatement(new WhileStatement(null!, null!, Filename, SyntaxNodeSpan.Empty));

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
        var ast = new Statements(Filename, SyntaxNodeSpan.Empty);
        ast.AddStatement(new IfElseStatement(null!, null!, null, Filename, SyntaxNodeSpan.Empty));
        ast.AddStatement(new ReturnStatement(null, Filename, SyntaxNodeSpan.Empty));
        ast.AddStatement(new WhileStatement(null!, null!, Filename, SyntaxNodeSpan.Empty));

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
        var ast = new Statements(Filename, SyntaxNodeSpan.Empty);
        ast.AddStatement(
            new IfElseStatement(
                new IntValueExpression(1, Filename, SyntaxNodeSpan.Empty),
                new BlockStatement(
                    new List<IStatement>
                    {
                        new ExpressionStatement(
                            new FunctionCallExpression(
                                new VariableExpression("hello", Filename, SyntaxNodeSpan.Empty),
                                [], Filename, SyntaxNodeSpan.Empty
                            ), Filename, SyntaxNodeSpan.Empty
                        ),
                        new ReturnStatement(null, Filename, SyntaxNodeSpan.Empty),
                        new WhileStatement(null!, null!, Filename, SyntaxNodeSpan.Empty)
                    }, Filename, SyntaxNodeSpan.Empty
                ),
                null, Filename, SyntaxNodeSpan.Empty
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