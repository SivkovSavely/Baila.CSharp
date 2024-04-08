using Baila.CSharp.Ast.Statements;

namespace Baila.CSharp.Parser.Tests;

public class ParserTests
{
    [Fact]
    public void IfElseStatementTest_TrueBranchOnly_EmptyTrueBranch()
    {
        var statements = ParseSource("""
                                    if expr {}
                                    """);
        var statement = statements.First();

        var ifElseStatement = Assert.IsType<IfElseStatement>(statement);
        var trueStatement = Assert.IsType<BlockStatement>(ifElseStatement.TrueStatement);
        Assert.Empty(trueStatement.Statements);
        Assert.Null(ifElseStatement.FalseStatement);
    }

    [Fact]
    public void IfElseStatementTest_TrueBranchAndFalseBranch_EmptyBranches()
    {
        var statements = ParseSource("""
                                    if expr {} else {}
                                    """);
        var statement = statements.First();

        var ifElseStatement = Assert.IsType<IfElseStatement>(statement);
        var trueStatement = Assert.IsType<BlockStatement>(ifElseStatement.TrueStatement);
        Assert.Empty(trueStatement.Statements);
        var falseStatement = Assert.IsType<BlockStatement>(ifElseStatement.FalseStatement);
        Assert.Empty(falseStatement.Statements);
    }

    private IStatement[] ParseSource(string source)
    {
        var lexer = new Lexer.Lexer(source, "test.baila");
        var parser = new Parser(lexer.Tokenize());
        var astRoot = parser.BuildAst();
        Assert.IsType<BlockStatement>(astRoot);
        return (astRoot as BlockStatement)!.Statements.ToArray();
    }
}