using Baila.CSharp.Typing;

namespace Baila.CSharp.Ast.Expressions;

public class VariableExpression(string name) : IExpression
{
    public string Name { get; } = name;

    public string Stringify()
    {
        return Name;
    }

    public BailaType? GetBailaType()
    {
        return null;
    }

    public override string ToString()
    {
        return $"VariableExpression({Name})";
    }
}