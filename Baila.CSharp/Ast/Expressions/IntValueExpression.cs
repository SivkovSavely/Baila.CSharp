using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;

namespace Baila.CSharp.Ast.Expressions;

public class IntValueExpression(int value) : IExpression
{
    public int Value { get; } = value;

    public BailaType GetBailaType() => BailaType.Int;

    public IValue Evaluate()
    {
        return new IntValue(value);
    }

    public string Stringify()
    {
        return Value.ToString();
    }

    public override string ToString()
    {
        return $"IntValueExpression({Value})";
    }
}