using System.Reflection;
using System.Text;
using Baila.CSharp.Ast.Diagnostics;
using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Ast.Syntax.Statements;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Types;
using Baila.CSharp.Runtime.Values;
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
        CompileTimeNameTable.CurrentScope = new CompileTimeNameTable.Scope();
        Formatter.AddFormatter(new ToStringFormatter());
    }

    static TestsBase()
    {
        BuiltInBailaTypes = typeof(BailaType)
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(f => f.FieldType == typeof(BailaType))
            .ToArray();
    }

    protected static Statements CompileProgram(string source, string filename = "test.baila",
        bool shouldFailTest = true)
    {
        try
        {
            return Repl.Interpreter.Compile(source, filename);
        }
        catch (ParseException e)
        {
            if (shouldFailTest)
            {
                var errorSb = new StringBuilder();
                errorSb.Append($"ParseException was caught with {e.Diagnostics.Count()} diagnostics");

                if (e.Diagnostics.Any())
                {
                    errorSb.AppendLine();
                }

                foreach (var diagnostic in e.Diagnostics)
                {
                    errorSb.Append($"[{diagnostic.GetCode()}] ");
                    errorSb.AppendLine(diagnostic.GetErrorMessage());

                    errorSb.AppendLine(diagnostic.GetFilename());

                    var maxLines = source.Split("\n").Length.ToString().Length;

                    foreach (var diagnosticLineSpan in diagnostic.GetLines())
                    {
                        var extraSpaces = maxLines - diagnosticLineSpan.LineNumber.ToString().Length;

                        var lineOffset = 2 + maxLines;

                        errorSb.Append("  ");
                        errorSb.Append(new string(' ', extraSpaces));
                        errorSb.Append(diagnosticLineSpan.LineNumber);
                        errorSb.Append(" | ");
                        errorSb.AppendLine(diagnosticLineSpan.FullLine);

                        errorSb.Append(new string(' ', lineOffset));
                        errorSb.Append(" | ");
                        errorSb.Append(new string(' ', diagnosticLineSpan.StartColumn - 1));
                        errorSb.Append(new string('^', diagnosticLineSpan.Length));

                        if (diagnosticLineSpan.Length == 1)
                        {
                            errorSb.AppendLine(
                                $"-- {diagnosticLineSpan.StartColumn}");
                        }
                        else
                        {
                            errorSb.AppendLine(
                                $"-- {diagnosticLineSpan.StartColumn}..{diagnosticLineSpan.StartColumn + diagnosticLineSpan.Length - 1}");
                        }
                    }
                }

                Assert.Fail(errorSb.ToString());
            }

            throw;
        }
    }

    protected static IValue? RunProgram(string source, string filename = "test.baila")
    {
        var program = CompileProgram(source, filename);
        program.Execute();
        return program.LastEvaluatedValue;
    }

    protected static void CompileProgramAndAssertError<TError>(string source, string? optionalMessage = null,
        string filename = "test.baila")
        where TError : Exception
    {
        var exception = Assert.Throws<TError>(() => CompileProgram(source, filename));
        if (optionalMessage != null)
        {
            Assert.Equal(optionalMessage, exception.Message);
        }
    }

    protected static void CompileProgramAndAssertDiagnosticExists(string source,
        Func<IDiagnostic, bool> diagnosticMatcher,
        string filename = "test.baila")
    {
        try
        {
            CompileProgram(source, filename, shouldFailTest: false);
        }
        catch (ParseException e)
        {
            if (e.Diagnostics.Any(diagnosticMatcher))
            {
                return;
            }

            Assert.Fail("None of the diagnostics matched");
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

    protected static void RunProgramAndAssertDiagnosticExists(string source,
        Func<IDiagnostic, bool> diagnosticMatcher,
        string filename = "test.baila")
    {
        try
        {
            var program = CompileProgram(source, filename, shouldFailTest: false);
            program.Execute();
        }
        catch (ParseException e)
        {
            if (e.Diagnostics.Any(diagnosticMatcher))
            {
                return;
            }

            Assert.Fail("None of the diagnostics matched");
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
        return BuiltInBailaTypes.FirstOrDefault(f => f.Name == typeName)?.GetValue(null) as BailaType ??
               new BailaType(typeName);
    }
}

file class ToStringFormatter : IValueFormatter
{
    public bool CanHandle(object? value)
    {
        // You can add additional logic here to only handle specific types
        return value is BailaType;
    }

    public void Format(object value, FormattedObjectGraph formattedGraph, FormattingContext context,
        FormatChild formatChild)
    {
        // Use the object's ToString method for formatting
        formattedGraph.AddFragment(value.ToString());
    }
}