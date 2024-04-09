using Baila.CSharp.Ast.Values;

namespace Baila.CSharp.Ast.Expressions;

public class ValueExpression(IValue value) : IExpression
{
    public string Stringify()
    {
        return value.GetAsString();
    }
}