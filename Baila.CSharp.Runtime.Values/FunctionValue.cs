using Baila.CSharp.Ast.Functional;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;

namespace Baila.CSharp.Runtime.Values;

public class FunctionValue(string name = "") : IValue
{
    private readonly List<FunctionOverload> _overloads = [];

    public IEnumerable<FunctionOverload> Overloads => _overloads.AsReadOnly();

    public static FunctionValue WithOverload(FunctionOverload overload, string name = "")
    {
        var value = new FunctionValue(name);
        value._overloads.Add(overload);
        return value;
    }
    
    public long GetAsInteger()
    {
        throw new InvalidOperationException("Cannot convert function to Int");
    }

    public double GetAsFloat()
    {
        throw new InvalidOperationException("Cannot convert function to Float");
    }

    public bool GetAsBoolean()
    {
        throw new InvalidOperationException("Cannot convert function to Bool");
    }

    public string GetAsString()
    {
        return name == "" ? "[function]" : $"[function {name}]";
    }

    public BailaType GetBailaType()
    {
        return BailaType.Function;
    }

    public override string ToString()
    {
        return $"FunctionValue(Name={name})";
    }
}