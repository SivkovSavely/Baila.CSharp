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
        return WithOverloads([overload], name);
    }

    public static FunctionValue WithOverloads(List<FunctionOverload> overloads, string name = "")
    {
        var value = new FunctionValue(name);
        foreach (var overload in overloads)
        {
            if (IsRequiredParameterAfterOptionalParameter(
                    overload,
                    out var requiredParameter,
                    out var optionalParameter))
            {
                throw new Exception($"In function '{name}', " +
                                    $"required parameter '{requiredParameter!.Name}' " +
                                    $"cannot be after an optional parameter '{optionalParameter!.Name}'");
            }

            value._overloads.Add(overload);
        }
        return value;
    }
    
    public static List<FunctionOverload> GetApplicableOverloads(IEnumerable<FunctionOverload> overloads, BailaType[] argTypes)
    {
        var applicableOverloads = new List<FunctionOverload>();

        foreach (var overload in overloads)
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
                applicableOverloads.Add(overload);
                break;
            }

            // Match where some of the arguments in the function are implicitly convertible to respecting parameter type
            if (argTypes.Select((t,i) => (t,i)).All(e => e.t.IsImplicitlyConvertibleTo(overload.Parameters[e.i].Type)))
            {
                applicableOverloads.Add(overload);
                break;
            }
        }

        return applicableOverloads;
    }

    public void AddOverload(FunctionOverload overload)
    {
        _overloads.Add(overload);
    }

    public bool HasOverload(FunctionOverload overload)
    {
        var applicableOverloads = GetApplicableOverloads(
            Overloads,
            overload.Parameters.Select(par => par.Type).ToArray());
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

    public static bool IsRequiredParameterAfterOptionalParameter(
        FunctionOverload overload,
        out FunctionParameter? requiredParameter,
        out FunctionParameter? optionalParameter)
    {
        if (overload.Parameters.Count == 0)
        {
            requiredParameter = null;
            optionalParameter = null;
            return false;
        }

        var seenOptionalParameter = false;
        FunctionParameter? firstOptionalParameter = null;
        foreach (var parameter in overload.Parameters)
        {
            if (parameter.DefaultValue == null && seenOptionalParameter)
            {
                requiredParameter = parameter;
                optionalParameter = firstOptionalParameter;
                return true;
            }

            if (parameter.DefaultValue != null)
            {
                seenOptionalParameter = true;
                firstOptionalParameter = parameter;
            }
        }

        requiredParameter = null;
        optionalParameter = null;
        return false;
    }
}