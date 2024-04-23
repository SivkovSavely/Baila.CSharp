using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;

namespace Baila.CSharp.Ast.Expressions;

public class StringValueExpression(string value) : IExpression
{
    public string Value { get; } = value;

    public BailaType GetBailaType() => BailaType.String;

    public IValue Evaluate()
    {
        return new StringValue(value);
    }

    public string Stringify()
    {
        return Value;
    }

    public override string ToString()
    {
        return $"StringValueExpression({Value})";
    }
}