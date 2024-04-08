using Baila.CSharp.Ast.Values;

namespace Baila.CSharp.Ast.Expressions;

public class ValueExpression(IValue value) : IExpression
{
    public IValue Evaluate()
    {
        return value;
    }

    public string Stringify()
    {
        return value.GetAsString();
    }
}