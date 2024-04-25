using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Expressions;

public class AssignmentExpression(string target, IExpression expression) : IExpression
{
    public string Target { get; } = target;
    public IExpression Expression { get; } = expression;

    public BailaType? GetBailaType()
    {
        return Expression.GetBailaType();
    }

    public IValue Evaluate()
    {
        var value = Expression.Evaluate();
        var member = NameTable.Get(Target);
        
        if (!Expression.GetBailaType()!.IsImplicitlyConvertibleTo(member.Type))
        {
            throw new Exception($"Cannot convert '{Expression.GetBailaType()}' to '{member.Type}'");
        }
        
        NameTable.Set(Target, value);
        return value;
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitAssignmentExpression(this);
    }

    public string Stringify()
    {
        return $"{Target} = {Expression.Stringify()}";
    }

    public override string ToString()
    {
        return $"AssignmentExpression({Target} = {Expression})";
    }
}