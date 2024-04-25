using System.Linq.Expressions;
using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Ast.Statements;

namespace Baila.CSharp.Tests.Infrastructure;

public class StatementStructureAsserter(IEnumerable<IStatement> statements) : IDisposable
{
    // This field is incremented on each Assert call.
    int _currentStatement;
    // The Statements object that is currently being asserted is passed in there too.
    readonly IStatement[] _currentStatements = statements.Where(x => x is not NoOpStatement).ToArray();

    /// <summary>
    /// Asserts that the Nth statement in the AST is of type TStmt. Also runs assertInner to assert inner structure of the statement.
    /// </summary>
    public void AssertRootStmt<TStmt>(Action<TStmt>? assertInner = null) where TStmt : IStatement
    {
        try
        {
            if (_currentStatement >= _currentStatements.Length)
            {
                Assert.Fail($"Extra statement was asserted: {typeof(TStmt).Name}");
            }
            var statement = _currentStatements[_currentStatement];
            var stmt = Assert.IsType<TStmt>(statement);
            assertInner?.Invoke(stmt);
        }
        finally
        {
            _currentStatement++;
        }
    }

    /// <summary>
    /// Asserts that the Nth statement in the AST is an expression statement of type TExpr. Also runs assertInner to assert inner structure of the expression statement.
    /// </summary>
    public void AssertRootExprStmt<TExpr>(Action<TExpr>? assertInner = null) where TExpr : IExpression
    {
        try
        {
            if (_currentStatement >= _currentStatements.Length)
            {
                Assert.Fail($"Extra statement was asserted: expression statement of type {typeof(TExpr).Name}");
            }
            var statement = _currentStatements[_currentStatement];
            var exprStmt = Assert.IsType<ExpressionStatement>(statement);
            var expr = Assert.IsType<TExpr>(exprStmt.Expression);
            assertInner?.Invoke(expr);
        }
        finally
        {
            _currentStatement++;
        }
    }

    public void AssertInner<TExpr>(IExpression expr, Expression<Func<TExpr, bool>>? assertFunc = null) where TExpr : IExpression
    {
        var expression = Assert.IsType<TExpr>(expr);
        if (assertFunc != null)
        {
            Assert.True(
                assertFunc.Compile()(expression),
                $"Expected '{assertFunc.Body.ToString().Trim('(', ')')}' to return true");
        }
    }

    public void AssertInner<TExpr>(IStatement stmt, Expression<Func<TExpr, bool>>? assertFunc = null) where TExpr : IExpression
    {
        var exprStatement = Assert.IsType<ExpressionStatement>(stmt);
        var expression = Assert.IsType<TExpr>(exprStatement.Expression);
        AssertInner(expression, assertFunc);
    }

    public void AssertInnerBlock(IStatement stmt, Action<BlockStatement, StatementStructureAsserter>? assertInner = null)
    {
        var blockStatement = Assert.IsType<BlockStatement>(stmt);
        var statements = blockStatement.Statements;

        using var innerAsserter = new StatementStructureAsserter(statements);
        assertInner?.Invoke(blockStatement, innerAsserter);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        if (_currentStatement < _currentStatements.Length)
        {
            var unassertedStatements = _currentStatements.Skip(_currentStatement).Select(s => s.ToString());
            Assert.Fail(
                $"There are {_currentStatements.Length - _currentStatement} more unasserted statements:\n{string.Join("\n", unassertedStatements)}");
        }
    }
}