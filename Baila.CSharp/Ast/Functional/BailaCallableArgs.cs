using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;

namespace Baila.CSharp.Ast.Functional;

public class BailaCallableArgs
{
    private readonly Dictionary<string, IValue> _argsByName = [];
    private readonly List<IValue> _argsByIndex = [];

    public IReadOnlyDictionary<string, IValue> ArgsByName => _argsByName.AsReadOnly();
    public IEnumerable<IValue> ArgsByIndex => _argsByIndex.AsReadOnly();
    public BailaType[] ArgumentTypes => _argsByIndex.Select(arg => arg.GetBailaType()).ToArray();

    public void AddArgument(string name, IValue value)
    {
        _argsByName[name] = value;
        _argsByIndex.Add(value);
    }

    public IValue Get(int argIndex)
    {
        return _argsByIndex[argIndex];
    }

    public IValue Get(string argName)
    {
        return _argsByName[argName];
    }

    public string GetString(int argIndex)
    {
        return _argsByIndex[argIndex].GetAsString();
    }

    public string GetString(string argName)
    {
        return _argsByName[argName].GetAsString();
    }

    public long GetInteger(int argIndex)
    {
        return _argsByIndex[argIndex].GetAsInteger();
    }

    public long GetInteger(string argName)
    {
        return _argsByName[argName].GetAsInteger();
    }

    public double GetFloat(int argIndex)
    {
        return _argsByIndex[argIndex].GetAsFloat();
    }

    public double GetFloat(string argName)
    {
        return _argsByName[argName].GetAsFloat();
    }

    public bool GetBoolean(int argIndex)
    {
        return _argsByIndex[argIndex].GetAsBoolean();
    }

    public bool GetBoolean(string argName)
    {
        return _argsByName[argName].GetAsBoolean();
    }
    
}