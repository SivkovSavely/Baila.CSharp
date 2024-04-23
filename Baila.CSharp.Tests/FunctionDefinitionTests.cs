using Baila.CSharp.Ast.Statements;

namespace Baila.CSharp.Tests;

public class FunctionDefinitionTests
{
    [Fact]
    public void BraceOnOtherLine_ShouldParse()
    {
        ParseSource("""
                    function test() : Int
                    {
                    }
                    """);
    }

    [Fact]
    public void BraceOnSameLine_ShouldParse()
    {
        ParseSource("""
                    function test() : Int {
                    }
                    """);
    }

    [Fact]
    public void ReturnTypeOnOtherLine1_ShouldParse()
    {
        ParseSource("""
                    function test()
                        : Int {}
                    """);
    }

    [Fact]
    public void ReturnTypeOnOtherLine2_ShouldParse()
    {
        ParseSource("""
                    function test() :
                        Int {}
                    """);
    }

    [Fact]
    public void NoParametersNoParentheses_ShouldParse()
    {
        ParseSource("""
                    function test : Int {}
                    """);
    }

    private IStatement[] ParseSource(string source)
    {
        var lexer = new Lexer.Lexer(source, "test.baila");
        var parser = new Parser.Parser(lexer.Tokenize());
        var astRoot = parser.BuildAst();
        Assert.IsType<BlockStatement>(astRoot);
        return astRoot.StatementList.ToArray();
    }
}