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