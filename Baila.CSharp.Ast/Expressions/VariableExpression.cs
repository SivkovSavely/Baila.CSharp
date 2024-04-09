namespace Baila.CSharp.Ast.Expressions;

public class VariableExpression(string name) : IExpression
{
    public string Stringify()
    {
        return name;
    }

    public override string ToString()
    {
        return $"VariableExpression({name})";
    }
}