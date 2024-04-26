using System.Reflection;
using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using FluentAssertions.Formatting;
using Xunit.Abstractions;

namespace Baila.CSharp.Tests.Infrastructure;

[Collection("TestsBase")]
public class TestsBase
{
    protected static readonly FieldInfo[] BuiltInBailaTypes;

    protected TestsBase(ITestOutputHelper testOutputHelper)
    {
        NameTable.CurrentScope = new NameTable.Scope();
        Formatter.AddFormatter(new ToStringFormatter());
    }

    static TestsBase()
    {
        BuiltInBailaTypes = typeof(BailaType)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(BailaType))
            .ToArray();
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

    protected static void CompileProgramAndAssertError<TError>(string source, string? optionalMessage = null, string filename = "test.baila")
        where TError : Exception
    {
        var exception = Assert.Throws<TError>(() => CompileProgram(source, filename));
        if (optionalMessage != null)
        {
            Assert.Equal(optionalMessage, exception.Message);
        }
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

    protected static bool IsExprFunctionCall(IExpression expr, string name)
    {
        return expr is FunctionCallExpression { FunctionHolder: VariableExpression ve } && ve.Name == name;
    }

    protected static bool IsStmtFunctionCall(IStatement stmt, string name)
    {
        return stmt is ExpressionStatement exprStmt && IsExprFunctionCall(exprStmt.Expression, name);
    }

    protected static BailaType GetBailaTypeByName(string typeName)
    {
        return BuiltInBailaTypes.FirstOrDefault(f => f.Name == typeName)?.GetValue(null) as BailaType ?? new BailaType(typeName);
    }
}

file class ToStringFormatter : IValueFormatter
{
    public bool CanHandle(object? value)
    {
        // You can add additional logic here to only handle specific types
        return value is not null; 
    }

    public void Format(object value, FormattedObjectGraph formattedGraph, FormattingContext context, FormatChild formatChild)
    {
        // Use the object's ToString method for formatting
        formattedGraph.AddFragment(value.ToString());
    }
}