using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Ast.Statements;

namespace Baila.CSharp.Visitors;

public abstract class VisitorBase
{
    #region Expressions

    public virtual void VisitAssignmentExpression(AssignmentExpression expr)
    {
        expr.Expression.AcceptVisitor(this);
    }

    public virtual void VisitBinaryExpression(BinaryExpression expr)
    {
        expr.Left.AcceptVisitor(this);
        expr.Right.AcceptVisitor(this);
    }

    public virtual void VisitBoolValueExpression(BoolValueExpression expr)
    {
    }

    public virtual void VisitFloatValueExpression(FloatValueExpression expr)
    {
    }

    public virtual void VisitFunctionCallExpression(FunctionCallExpression expr)
    {
        expr.FunctionHolder.AcceptVisitor(this);
        foreach (var callArg in expr.CallArgs)
        {
            callArg.AcceptVisitor(this);
        }
    }

    public virtual void VisitIntValueExpression(IntValueExpression expr)
    {
    }

    public virtual void VisitPrefixUnaryExpression(PrefixUnaryExpression expr)
    {
        expr.OperandExpression.AcceptVisitor(this);
    }

    public virtual void VisitStringValueExpression(StringValueExpression expr)
    {
    }

    public virtual void VisitTypeOfExpression(TypeOfExpression expr)
    {
        expr.Expression.AcceptVisitor(this);
    }

    public virtual void VisitVariableExpression(VariableExpression expr)
    {
    }

    #endregion

    #region Statements

    public virtual void VisitBlockStatement(BlockStatement stmt)
    {
        foreach (var innerStatement in stmt.Statements)
        {
            innerStatement.AcceptVisitor(this);
        }
    }

    public virtual void VisitConstantDefineStatement(ConstantDefineStatement stmt)
    {
        stmt.Value.AcceptVisitor(this);
    }

    public virtual void VisitDoWhileStatement(DoWhileStatement stmt)
    {
        stmt.Condition.AcceptVisitor(this);
        stmt.Body.AcceptVisitor(this);
    }

    public virtual void VisitExpressionStatement(ExpressionStatement stmt)
    {
        stmt.Expression.AcceptVisitor(this);
    }

    public virtual void VisitForStatement(ForStatement stmt)
    {
        stmt.InitialValue.AcceptVisitor(this);
        stmt.FinalValue.AcceptVisitor(this);
        stmt.StepValue.AcceptVisitor(this);
        stmt.Body.AcceptVisitor(this);
    }

    public virtual void VisitFunctionDefineStatement(FunctionDefineStatement stmt)
    {
        stmt.Body.AcceptVisitor(this);
    }

    public virtual void VisitIfElseStatement(IfElseStatement stmt)
    {
        stmt.Condition.AcceptVisitor(this);
        stmt.TrueStatement.AcceptVisitor(this);
        stmt.FalseStatement?.AcceptVisitor(this);
    }

    public virtual void VisitNoOpStatement(NoOpStatement stmt)
    {
    }

    public virtual void VisitReturnStatement(ReturnStatement stmt)
    {
        stmt.ReturnExpression?.AcceptVisitor(this);
    }

    public virtual void VisitStatements(Statements stmt)
    {
        foreach (var innerStatement in stmt.StatementList)
        {
            innerStatement.AcceptVisitor(this);
        }
    }

    public virtual void VisitVariableDefineStatement(VariableDefineStatement stmt)
    {
        stmt.ValueExpression?.AcceptVisitor(this);
    }

    public virtual void VisitWhileStatement(WhileStatement stmt)
    {
        stmt.Condition.AcceptVisitor(this);
        stmt.Body.AcceptVisitor(this);
    }

    #endregion
}