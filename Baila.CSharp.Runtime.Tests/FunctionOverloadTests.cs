using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Interpreter.StatementInterpreters;
using Xunit.Abstractions;

namespace Baila.CSharp.Runtime.Tests;

public class FunctionOverloadTests(ITestOutputHelper testOutputHelper)
{
    [Fact]
    public void FunctionWithOneOverload_CallSuccessful()
    {
        var ast = GetAst("""
                         function testFunc(x: Int) : Int
                         {
                         }
                         
                         testFunc(123)
                         """);
        
        ast.InterpretExecute();
    }

    private static IStatement GetAst(string source)
    {
        var lexer = new Lexer.Lexer(source, "test.baila");
        var parser = new Parser.Parser(lexer.Tokenize());
        return parser.BuildAst();
    }
}