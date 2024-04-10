using Baila.CSharp.Typing;

namespace Baila.CSharp.Runtime.Values;

public class BooleanValue(bool value) : IValue
{
    public long GetAsInteger()
    {
        return value ? 1 : 0;
    }

    public double GetAsFloat()
    {
        return value ? 1 : 0;
    }

    public bool GetAsBoolean()
    {
        return value;
    }

    public string GetAsString()
    {
        return value ? "true" : "false";
    }

    public BailaType GetBailaType()
    {
        return BailaType.Bool;
    }

    public override string ToString()
    {
        return $"BooleanValue({value})";
    }
}