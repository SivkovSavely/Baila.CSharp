using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;

namespace Baila.CSharp.Ast.Expressions;

public class AssignmentExpression(string target, IExpression expression) : IExpression
{
    public BailaType? GetBailaType()
    {
        return expression.GetBailaType();
    }

    public IValue Evaluate()
    {
        var value = expression.Evaluate();
        var member = NameTable.Get(target);
        
        if (!expression.GetBailaType()!.IsImplicitlyConvertibleTo(member.Type))
        {
            throw new Exception($"Cannot convert '{expression.GetBailaType()}' to '{member.Type}'");
        }
        
        NameTable.Set(target, value);
        return value;
    }

    public string Stringify()
    {
        return $"{target} = {expression.Stringify()}";
    }

    public override string ToString()
    {
        return $"AssignmentExpression({target} = {expression})";
    }
}