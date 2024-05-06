using Baila.CSharp.Runtime.Types;

namespace Baila.CSharp.Runtime.Values;

public class IntValue(long value) : IValue
{
    public long GetAsInteger()
    {
        return value;
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
        return value.ToString();
    }

    public BailaType GetBailaType()
    {
        return BailaType.Int;
    }

    public override string ToString()
    {
        return $"IntValue({value})";
    }
}