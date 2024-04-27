using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public record VariableExpression(string Name) : IExpression
{
    public string Stringify()
    {
        return Name;
    }

    public BailaType GetBailaType()
    {
        var variable = NameTable.Get(Name);
        return variable.Type;
    }

    public IValue Evaluate()
    {
        var member = NameTable.Get(Name);
        return member.Value;
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitVariableExpression(this);
    }

    public override string ToString()
    {
        return $"VariableExpression({Name})";
    }
}