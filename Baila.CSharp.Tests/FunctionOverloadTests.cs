using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Interpreter.StatementInterpreters;
using Xunit.Abstractions;

namespace Baila.CSharp.Tests;

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

    [Fact]
    public void FunctionWithTwoOverloads_DifferByParameterCount_CallSuccessful()
    {
        var ast = GetAst("""
                         function testFunc(x: Int) : Int
                         {
                           return x
                         }
                         function testFunc(x: Int, y: Int) : Int
                         {
                           return y
                         }
                         
                         testFunc(123)
                         testFunc(123, 456)
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