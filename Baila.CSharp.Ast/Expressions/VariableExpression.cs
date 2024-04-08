using Baila.CSharp.Ast.Values;

namespace Baila.CSharp.Ast.Expressions;

public class VariableExpression(string name) : IExpression
{
    public IValue Evaluate()
    {
        // TODO access nametable
        return null!;
    }

    public string Stringify()
    {
        return name;
    }
}