using Baila.CSharp.Ast.Values;

namespace Baila.CSharp.Ast.Expressions;

public class AssignmentExpression(string target, IExpression expression) : IExpression
{
    public IValue Evaluate()
    {
        // TODO assign to variable
        return expression.Evaluate();
    }

    public string Stringify()
    {
        return $"{target} = {expression.Stringify()}";
    }
}