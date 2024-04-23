using Baila.CSharp.Typing;

namespace Baila.CSharp.Ast.Expressions;

public class BoolValueExpression(bool value) : IExpression
{
    public bool Value { get; } = value;

    public BailaType GetBailaType() => BailaType.Bool;

    public string Stringify()
    {
        return Value ? "true" : "false";
    }

    public override string ToString()
    {
        return $"BoolValueExpression({Value})";
    }
}