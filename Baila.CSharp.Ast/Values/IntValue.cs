namespace Baila.CSharp.Ast.Values;

public class IntValue(int value) : IValue
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
}