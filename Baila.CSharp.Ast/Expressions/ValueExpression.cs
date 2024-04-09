using Baila.CSharp.Ast.Values;

namespace Baila.CSharp.Ast.Expressions;

public class ValueExpression(IValue value) : IExpression
{
    public IValue Value { get; } = value;

    public string Stringify()
    {
        return Value.GetAsString();
    }

    public override string ToString()
    {
        return $"ValueExpression({Value})";
    }
}