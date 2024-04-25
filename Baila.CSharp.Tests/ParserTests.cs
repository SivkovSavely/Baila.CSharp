using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Tests.Infrastructure;
using Xunit.Abstractions;

namespace Baila.CSharp.Tests;

public class ParserTests : TestsBase
{
    public ParserTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public void IfElseStatementTest_TrueBranchOnly_EmptyTrueBranch()
    {
        var statements = CompileProgram("""
                                        if expr {}
                                        """);
        var statement = statements.StatementList.First();

        var ifElseStatement = Assert.IsType<IfElseStatement>(statement);
        var trueStatement = Assert.IsType<BlockStatement>(ifElseStatement.TrueStatement);
        Assert.Empty(trueStatement.Statements);
        Assert.Null(ifElseStatement.FalseStatement);
    }

    [Fact]
    public void IfElseStatementTest_TrueBranchAndFalseBranch_EmptyBranches()
    {
        var statements = CompileProgram("""
                                        if expr {} else {}
                                        """);
        var statement = statements.StatementList.First();

        var ifElseStatement = Assert.IsType<IfElseStatement>(statement);
        var trueStatement = Assert.IsType<BlockStatement>(ifElseStatement.TrueStatement);
        Assert.Empty(trueStatement.Statements);
        var falseStatement = Assert.IsType<BlockStatement>(ifElseStatement.FalseStatement);
        Assert.Empty(falseStatement.Statements);
    }
}