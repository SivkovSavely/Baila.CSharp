using System.Text;
using Baila.CSharp.Runtime.Types;

namespace Baila.CSharp.Runtime.Values;

public class StringValue(string value) : IValue
{
    public static StringValue FromRepeated(string value, long repetitionCount)
    {
        if (repetitionCount < 0)
        {
            throw new Exception("Repetition count cannot be less than zero");
        }

        var sb = new StringBuilder();

        for (var i = 0L; i < repetitionCount; i++)
        {
            sb.Append(value);
        }

        return new StringValue(sb.ToString());
    }
    
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