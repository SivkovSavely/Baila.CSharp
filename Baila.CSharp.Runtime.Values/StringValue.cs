using Baila.CSharp.Typing;

namespace Baila.CSharp.Runtime.Values;

public class StringValue(string value) : IValue
{
    public long GetAsInteger()
    {
        return long.Parse(value);
    }

    public double GetAsFloat()
    {
        return double.Parse(value);
    }

    public bool GetAsBoolean()
    {
        return value.Length != 0;
    }

    public string GetAsString()
    {
        return value;
    }

    public BailaType GetBailaType()
    {
        return BailaType.String;
    }

    public override string ToString()
    {
        return $"StringValue({value})";
    }
}