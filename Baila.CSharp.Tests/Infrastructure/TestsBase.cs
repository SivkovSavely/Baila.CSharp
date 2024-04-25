using System.Reflection;
using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values.Abstractions;
using Xunit.Abstractions;

namespace Baila.CSharp.Tests.Infrastructure;

[Collection("TestsBase")]
public class TestsBase
{
    protected TestsBase(ITestOutputHelper testOutputHelper)
    {
        var test = testOutputHelper.GetType()
            .GetField("test", BindingFlags.Instance | BindingFlags.NonPublic)!
            .GetValue(testOutputHelper)! as ITest;

        testOutputHelper.WriteLine($"Reset nametable in {test!.DisplayName}");
        
        NameTable.CurrentScope = new NameTable.Scope();
    }
    
    protected static Statements CompileProgram(string source, string filename = "test.baila")
    {
        return Repl.Interpreter.Compile(source, filename);
    }

    protected static IValue? RunProgram(string source, string filename = "test.baila")
    {
        var program = CompileProgram(source, filename);
        program.Execute();
        return program.LastEvaluatedValue;
    }

    protected static void RunProgramAndAssertError<TError>(Statements program, string? optionalMessage = null)
        where TError : Exception
    {
        var exception = Assert.Throws<TError>(program.Execute);
        if (optionalMessage != null)
        {
            Assert.Equal(optionalMessage, exception.Message);
        }
    }

    protected static void AssertValue<TValue>(IValue value, Func<TValue, bool>? assertCondition = null)
        where TValue : IValue
    {
        var tValue = Assert.IsType<TValue>(value);
        if (assertCondition != null)
        {
            Assert.True(assertCondition(tValue));
        }
    }

    protected static void AssertAst(Statements program, Action<StatementStructureAsserter> asserterSequence)
    {
        using var asserter = new StatementStructureAsserter(program.StatementList);
        asserterSequence(asserter);
    }
}