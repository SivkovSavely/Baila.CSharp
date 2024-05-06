using Baila.CSharp.Ast.Diagnostics;
using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Ast.Syntax;
using Baila.CSharp.Ast.Syntax.Statements;
using Baila.CSharp.Interpreter;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values;

namespace Baila.CSharp.Visitors;

public class CompileTimeFunctionDefiningVisitor(List<IDiagnostic> diagnostics, string[] sourceLines) : VisitorBase
{
    public List<IDiagnostic> Diagnostics { get; } = diagnostics;

    public override void VisitFunctionDefineStatement(FunctionDefineStatement stmt)
    {
        if (CompileTimeNameTable.ExistsVariable(stmt.Name))
        {
            Diagnostics.Add(
                ParserDiagnostics.BP0008_VariableIsNotAFunction(
                    stmt.Name,
                    stmt,
                    GetRelevantSourceLines(stmt.Span)));
            return;
        }
        
        var overload = new FunctionOverload(
            new StatementCallable(stmt.Body),
            stmt.Parameters,
            stmt.ReturnType);
        
        if (FunctionValue.IsRequiredParameterAfterOptionalParameter(
                overload,
                out var requiredParameter,
                out var optionalParameter))
        {
            Diagnostics.Add(
                ParserDiagnostics.BP0013_RequiredParametersShouldBeBeforeOptionalParameters(
                    stmt.Name,
                    requiredParameter!.Name,
                    optionalParameter!.Name,
                    stmt, // TODO better to underline parameters only instead of the whole function
                    GetRelevantSourceLines(stmt.Span)));
        }
        else if (!CompileTimeNameTable.TryAddFunction(stmt.Name, overload, out var conflictingOverload))
        {
            Diagnostics.Add(
                ParserDiagnostics.BP0007_OverloadExists(
                    stmt,
                    overload,
                    conflictingOverload!,
                    GetRelevantSourceLines(stmt.Span)));
        }

        foreach (var parameter in stmt.Parameters)
        {
            CompileTimeNameTable.AddVariable(parameter.Name, parameter.Type);
        }

        base.VisitFunctionDefineStatement(stmt);
    }

    private string[] GetRelevantSourceLines(SyntaxNodeSpan span)
    {
        return sourceLines[(span.StartLine - 1)..span.EndLine];
    }
}