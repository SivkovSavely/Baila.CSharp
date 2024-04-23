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

    public static FunctionValue WithOverloads(List<FunctionOverload> overloads, string name = "")
    {
        var value = new FunctionValue(name);
        foreach (var overload in overloads)
        {
            value._overloads.Add(overload);
        }
        return value;
    }
    
    public List<FunctionOverload> GetApplicableOverloads(BailaType[] argTypes)
    {
        var found = new List<FunctionOverload>();

        foreach (var overload in Overloads)
        {
            // If we passed fewer arguments than the required parameters count, skip that overload
            if (argTypes.Length < overload.Parameters.Count(par => par.DefaultValue == null))
            {
                continue;
            }

            // If we passed more arguments than the total parameters count, skip that overload
            if (argTypes.Length > overload.Parameters.Count)
            {
                continue;
            }
            
            // Best match
            if (argTypes.SequenceEqual(overload.Parameters.Select(par => par.Type)))
            {
                found.Add(overload);
                break;
            }
        }

        return found;
    }

    public void AddOverload(FunctionOverload overload)
    {
        _overloads.Add(overload);
    }

    public bool HasOverload(FunctionOverload overload)
    {
        var applicableOverloads = GetApplicableOverloads(overload.Parameters.Select(par => par.Type).ToArray());
        return applicableOverloads.Count != 0;
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