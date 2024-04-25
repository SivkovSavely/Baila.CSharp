using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Statements;

public record ExpressionStatement(IExpression Expression) : IStatement
{
    public IValue? Value { get; set; }

    public void Execute()
    {
        Value = Expression.Evaluate();
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitExpressionStatement(this);
    }

    public override string ToString()
    {
        return $"ExpressionStatement(Expression={Expression})";
    }
}