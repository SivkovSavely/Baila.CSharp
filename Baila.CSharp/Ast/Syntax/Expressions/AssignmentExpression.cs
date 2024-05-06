using System.Diagnostics.CodeAnalysis;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public record AssignmentExpression(
    IExpression TargetExpression,
    ISyntaxNode EqualsSign,
    IExpression Expression) : IExpression
{
    public SyntaxNodeSpan Span { get; init; } = SyntaxNodeSpan.Merge(TargetExpression, EqualsSign, Expression);

    public BailaType? GetBailaType()
    {
        return Expression.GetBailaType();
    }

    public IValue Evaluate()
    {
        if (TargetExpression is not VariableExpression variableExpression)
        {
            throw new Exception($"AssignmentExpression can only evaluate VariableExpressions for now, {TargetExpression.GetType().Name} was passed instead");
        }
        
        var value = Expression.Evaluate();
        var member = NameTable.Get(variableExpression.Name);
        
        if (!Expression.GetBailaType()!.IsImplicitlyConvertibleTo(member.Type))
        {
            throw new Exception($"Cannot convert '{Expression.GetBailaType()}' to '{member.Type}'");
        }
        
        NameTable.Set(variableExpression.Name, value);
        return value;
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitAssignmentExpression(this);
    }

    [ExcludeFromCodeCoverage]
    public string Stringify()
    {
        return $"{TargetExpression.Stringify()} = {Expression.Stringify()}";
    }

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        return $"AssignmentExpression({TargetExpression} = {Expression})";
    }
}