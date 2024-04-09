namespace Baila.CSharp.Ast.Expressions;

public class AssignmentExpression(string target, IExpression expression) : IExpression
{
    public string Stringify()
    {
        return $"{target} = {expression.Stringify()}";
    }
}