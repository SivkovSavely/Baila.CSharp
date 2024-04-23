using Baila.CSharp.Typing;

namespace Baila.CSharp.Ast.Expressions;

public class StringValueExpression(string value) : IExpression
{
    public string Value { get; } = value;

    public BailaType GetBailaType() => BailaType.String;

    public string Stringify()
    {
        return Value;
    }

    public override string ToString()
    {
        return $"StringValueExpression({Value})";
    }
}