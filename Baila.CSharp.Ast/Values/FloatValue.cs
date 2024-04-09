using System.Globalization;
using Baila.CSharp.Typing;

namespace Baila.CSharp.Ast.Values;

public class FloatValue(double value) : IValue
{
    public long GetAsInteger()
    {
        return (long)value;
    }

    public double GetAsFloat()
    {
        return value;
    }

    public bool GetAsBoolean()
    {
        return value != 0;
    }

    public string GetAsString()
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    public BailaType GetBailaType()
    {
        return BailaType.Float;
    }

    public override string ToString()
    {
        return $"FloatValue({value})";
    }
}