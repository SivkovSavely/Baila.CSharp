using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Ast.Statements;

namespace Baila.CSharp.Visitors;

public abstract class VisitorBase
{
    #region Expressions

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

    #endregion

    #region Statements

    public void VisitBlockStatement(BlockStatement stmt)
    {
        foreach (var innerStatement in stmt.Statements)
        {
            innerStatement.AcceptVisitor(this);
        }
    }

    public void VisitConstantDefineStatement(ConstantDefineStatement stmt)
    {
        stmt.Value.AcceptVisitor(this);
    }

    public void VisitDoWhileStatement(DoWhileStatement stmt)
    {
        stmt.Condition.AcceptVisitor(this);
        stmt.Body.AcceptVisitor(this);
    }

    public void VisitExpressionStatement(ExpressionStatement stmt)
    {
        stmt.Expression.AcceptVisitor(this);
    }

    public void VisitForStatement(ForStatement stmt)
    {
        stmt.InitialValue.AcceptVisitor(this);
        stmt.FinalValue.AcceptVisitor(this);
        stmt.StepValue.AcceptVisitor(this);
        stmt.Body.AcceptVisitor(this);
    }

    public void VisitFunctionDefineStatement(FunctionDefineStatement stmt)
    {
        stmt.Body.AcceptVisitor(this);
    }

    public void VisitIfElseStatement(IfElseStatement stmt)
    {
        stmt.Condition.AcceptVisitor(this);
        stmt.TrueStatement.AcceptVisitor(this);
        stmt.FalseStatement?.AcceptVisitor(this);
    }

    public void VisitNoOpStatement(NoOpStatement stmt)
    {
    }

    public void VisitReturnStatement(ReturnStatement stmt)
    {
        stmt.ReturnExpression?.AcceptVisitor(this);
    }

    public void VisitStatements(Statements stmt)
    {
        foreach (var innerStatement in stmt.StatementList)
        {
            innerStatement.AcceptVisitor(this);
        }
    }

    public void VisitVariableDefineStatement(VariableDefineStatement stmt)
    {
        stmt.ValueExpression?.AcceptVisitor(this);
    }

    public void VisitWhileStatement(WhileStatement stmt)
    {
        stmt.Condition.AcceptVisitor(this);
        stmt.Body.AcceptVisitor(this);
    }

    #endregion
}