using Baila.CSharp.Ast.Expressions;

namespace Baila.CSharp.Visitors;

public abstract class VisitorBase
{
    public void VisitAssignmentExpression(AssignmentExpression expr)
    {
        expr.Expression.AcceptVisitor(this);
    }

    public void VisitBinaryExpression(BinaryExpression expr)
    {
        expr.Left.AcceptVisitor(this);
        expr.Right.AcceptVisitor(this);
    }

    public void VisitBoolValueExpression(BoolValueExpression expr)
    {
    }

    public void VisitFunctionCallExpression(FunctionCallExpression expr)
    {
        expr.FunctionHolder.AcceptVisitor(this);
        foreach (var callArg in expr.CallArgs)
        {
            callArg.AcceptVisitor(this);
        }
    }

    public void VisitIntValueExpression(IntValueExpression expr)
    {
    }

    public void VisitPrefixUnaryExpression(PrefixUnaryExpression expr)
    {
        expr.OperandExpression.AcceptVisitor(this);
    }

    public void VisitStringValueExpression(StringValueExpression expr)
    {
    }

    public void VisitTypeOfExpression(TypeOfExpression expr)
    {
        expr.Expression.AcceptVisitor(this);
    }

    public void VisitVariableExpression(VariableExpression expr)
    {
    }
}