namespace Baila.CSharp.Ast.Expressions;

public class IntValueExpression(int value) : IExpression
{
    public int Value { get; } = value;

    public string Stringify()
    {
        return Value.ToString();
    }

    public override string ToString()
    {
        return $"IntValueExpression({Value})";
    }
}