using Baila.CSharp.Interpreter.StatementInterpreters;
using Baila.CSharp.Interpreter.Stdlib;

namespace Baila.CSharp.Tests;

public class ForStatementTests
{
    public ForStatementTests()
    {
        NameTable.CurrentScope = new NameTable.Scope(); // clear scope
    }
    
    [Fact]
    public void X()
    {
        var lexer = new Lexer.Lexer("""
                                    for i = 1 to 3 {}
                                    """, "");
        var parser = new Parser.Parser(lexer.Tokenize());
        var stmt = parser.BuildAst();

        Assert.False(NameTable.Exists("i"), "Loop counter should not exist at this point in time");
        stmt.InterpretExecute();
        Assert.False(NameTable.Exists("i"), "Loop counter should not be exposed to the scope after loop ran");
    }
}