﻿using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Ast.Statements;
using Xunit.Sdk;

namespace Baila.CSharp.Tests.Infrastructure;

public class StatementStructureAsserterTests : TestsBase
{
    [Fact]
    public void CorrectAssertion_AssertsCorrectly()
    {
        var ast = new Statements();
        ast.AddStatement(
            new IfElseStatement(
                new IntValueExpression(1),
                new ExpressionStatement(
                    new FunctionCallExpression(
                        new VariableExpression("hello"),
                        []
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
                new IntValueExpression(1),
                new ExpressionStatement(
                    new FunctionCallExpression(
                        new VariableExpression("hello"),
                        []
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
                new IntValueExpression(1),
                new ExpressionStatement(
                    new FunctionCallExpression(
                        new VariableExpression("hello"),
                        []
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
                new IntValueExpression(1),
                new ExpressionStatement(
                    new FunctionCallExpression(
                        new VariableExpression("hello"),
                        []
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
        ast.AddStatement(new IfElseStatement(null!, null!, null));
        ast.AddStatement(new NoOpStatement());
        ast.AddStatement(new WhileStatement(null!, null!));

        AssertAst(
            ast,
            a =>
            {
                a.AssertRootStmt<IfElseStatement>(_ => { });
                a.AssertRootStmt<NoOpStatement>(_ => { });
                a.AssertRootStmt<WhileStatement>(_ => { });
            });
    }

    [Fact]
    public void SeveralStatements_SomeAreNotAsserted_TestFails()
    {
        var ast = new Statements();
        ast.AddStatement(new IfElseStatement(null!, null!, null));
        ast.AddStatement(new NoOpStatement());
        ast.AddStatement(new WhileStatement(null!, null!));

        var exception = Assert.ThrowsAny<XunitException>(() =>
        {
            AssertAst(
                ast,
                a =>
                {
                    a.AssertRootStmt<IfElseStatement>(_ => { });
                    a.AssertRootStmt<NoOpStatement>(_ => { });
                });
        });
        Assert.Equal("There are 1 more unasserted statements.", exception.Message);
    }

    [Fact]
    public void SeveralStatements_ExtraAreAsserted_TestFails()
    {
        var ast = new Statements();
        ast.AddStatement(new IfElseStatement(null!, null!, null));
        ast.AddStatement(new NoOpStatement());
        ast.AddStatement(new WhileStatement(null!, null!));

        var exception = Assert.ThrowsAny<XunitException>(() =>
        {
            AssertAst(
                ast,
                a =>
                {
                    a.AssertRootStmt<IfElseStatement>(_ => { });
                    a.AssertRootStmt<NoOpStatement>(_ => { });
                    a.AssertRootStmt<WhileStatement>(_ => { });
                    a.AssertRootStmt<NoOpStatement>(_ => { });
                });
        });
        Assert.Equal("Extra statement was asserted: NoOpStatement", exception.Message);
    }

    [Fact]
    public void AssertInnerBlock_CorrectAssertion_AssertsCorrectly()
    {
        var ast = new Statements();
        ast.AddStatement(
            new IfElseStatement(
                new IntValueExpression(1),
                new BlockStatement(
                    new List<IStatement>
                    {
                        new ExpressionStatement(
                            new FunctionCallExpression(
                                new VariableExpression("hello"),
                                []
                            )
                        ),
                        new NoOpStatement(),
                        new WhileStatement(null!, null!)
                    }
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
                                innerAsserter.AssertRootStmt<NoOpStatement>();
                                innerAsserter.AssertRootStmt<WhileStatement>();
                            });
                        Assert.Null(stmt.FalseStatement);
                    });
            });
    }
}