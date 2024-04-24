using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Runtime.Values.Abstractions;

namespace Baila.CSharp.Tests.Infrastructure;

public class TestsBase
{
    protected Statements CompileProgram(string source, string filename = "test.baila")
    {
        var lexer = new Lexer.Lexer(source, filename);
        var parser = new Parser.Parser(lexer.Tokenize());
        return parser.BuildAst();
    }

    protected IValue? RunProgram(string source, string filename = "test.baila")
    {
        var program = CompileProgram(source, filename);
        program.Execute();
        return program.LastEvaluatedValue;
    }

    protected void RunProgramAndAssertError<TError>(Statements program, string? optionalMessage = null)
        where TError : Exception
    {
        var exception = Assert.Throws<TError>(program.Execute);
        if (optionalMessage != null)
        {
            Assert.Equal(optionalMessage, exception.Message);
        }
    }

    protected void AssertValue<TValue>(IValue value, Func<TValue, bool>? assertCondition = null)
        where TValue : IValue
    {
        var tValue = Assert.IsType<TValue>(value);
        if (assertCondition != null)
        {
            Assert.True(assertCondition(tValue));
        }
    }
}