using Baila.CSharp.Ast.Diagnostics;
using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Ast.Syntax;
using Baila.CSharp.Ast.Syntax.Expressions;
using Baila.CSharp.Ast.Syntax.Statements;
using Baila.CSharp.Interpreter;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Parser;

public class TypeCheckingVisitor(List<IDiagnostic> diagnostics, string[] sourceLines) : VisitorBase
{
    public List<IDiagnostic> Diagnostics { get; } = diagnostics;

    public override void VisitVariableDefineStatement(VariableDefineStatement stmt)
    {
        base.VisitVariableDefineStatement(stmt);
        if (CompileTimeNameTable.Exists(stmt.Name))
        {
            diagnostics.Add(
                ParserDiagnostics.BP0004_CannotRedefineVariable(
                    stmt,
                    GetRelevantSourceLines(stmt.Span)));
        }
        else
        {
            var type = stmt.Type ?? stmt.ValueExpression?.GetBailaType();

            if (type == null)
            {
                diagnostics.Add(
                    ParserDiagnostics.BP0005_CannotInferTypeOfVariable(
                        stmt,
                        GetRelevantSourceLines(stmt.Span)));
            }
            else
            {
                if (type == BailaType.Any && stmt.ValueExpression == null)
                {
                    diagnostics.Add(
                        ParserDiagnostics.BP0012_HaveToAssignValueToAnyVariable(
                            stmt.Name,
                            stmt,
                            GetRelevantSourceLines(stmt.Span)));
                }
                else
                {
                    CompileTimeNameTable.AddVariable(stmt.Name, type);

                    if (stmt.ValueExpression != null)
                    {
                        ValidateAssignmentToVariable(stmt.Name, stmt.ValueExpression, stmt.ValueExpression);
                    }
                }
            }
        }

    }

    public override void VisitConstantDefineStatement(ConstantDefineStatement stmt)
    {
        base.VisitConstantDefineStatement(stmt);
        if (CompileTimeNameTable.Exists(stmt.Name))
        {
            diagnostics.Add(
                ParserDiagnostics.BP0006_CannotRedefineConstant(
                    stmt,
                    GetRelevantSourceLines(stmt.Span)));
        }
        else
        {
            var type = stmt.Value.GetBailaType();
            CompileTimeNameTable.AddVariable(stmt.Name, type);
        }
    }

    public override void VisitFunctionDefineStatement(FunctionDefineStatement stmt)
    {
        base.VisitFunctionDefineStatement(stmt);
        if (CompileTimeNameTable.ExistsVariable(stmt.Name))
        {
            diagnostics.Add(
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

        if (!CompileTimeNameTable.TryAddFunction(stmt.Name, overload))
        {
            diagnostics.Add(
                ParserDiagnostics.BP0007_OverloadExists(
                    stmt,
                    overload,
                    GetRelevantSourceLines(stmt.Span)));
        }
    }

    public override void VisitFunctionCallExpression(FunctionCallExpression expr)
    {
        base.VisitFunctionCallExpression(expr);

        // TODO add call to BailaType.IsCallable
        if (expr.FunctionHolder.GetBailaType() != BailaType.Function)
        {
            diagnostics.Add(
                ParserDiagnostics.BP0010_SymbolIsNotDefined(
                    expr,
                    GetRelevantSourceLines(expr.Span)));
        }
    }

    public override void VisitVariableExpression(VariableExpression expr)
    {
        base.VisitVariableExpression(expr);
        if (!CompileTimeNameTable.Exists(expr.Name))
        {
            diagnostics.Add(
                ParserDiagnostics.BP0009_SymbolIsNotDefined(
                    expr.Name,
                    expr,
                    GetRelevantSourceLines(expr.Span)));
        }
    }

    public override void VisitAssignmentExpression(AssignmentExpression expr)
    {
        base.VisitAssignmentExpression(expr);
        if (expr.TargetExpression is VariableExpression targetVariableExpression)
        {
            if (!CompileTimeNameTable.Exists(targetVariableExpression.Name))
            {
                diagnostics.Add(
                    ParserDiagnostics.BP0009_SymbolIsNotDefined(
                        targetVariableExpression.Name,
                        targetVariableExpression,
                        GetRelevantSourceLines(targetVariableExpression.Span)));
            }
            else
            {
                ValidateAssignmentToVariable(targetVariableExpression.Name, expr.Expression, expr);
            }
        }
    }

    private void ValidateAssignmentToVariable(
        string targetName, IExpression assigningExpression, IExpression syntaxNode)
    {
        var member = CompileTimeNameTable.Get(targetName)!;
        var fromType = assigningExpression.GetBailaType()!;
        var toType = member.Type;
        if (!fromType.IsImplicitlyConvertibleTo(toType))
        {
            diagnostics.Add(
                ParserDiagnostics.BP0011_CannotConvertTypeFromAToB(
                    syntaxNode,
                    fromType,
                    toType,
                    GetRelevantSourceLines(syntaxNode.Span)));
        }
    }

    private string[] GetRelevantSourceLines(SyntaxNodeSpan span)
    {
        return sourceLines[(span.StartLine - 1)..span.EndLine];
    }
}