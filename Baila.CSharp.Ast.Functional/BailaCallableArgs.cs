using Baila.CSharp.Runtime.Values.Abstractions;

namespace Baila.CSharp.Ast.Functional;

public class BailaCallableArgs
{
    private readonly Dictionary<string, IValue> _argsByName = [];
    private readonly List<IValue> _argsByIndex = [];

    public void AddArgument(string name, IValue value)
    {
        _argsByName[name] = value;
        _argsByIndex.Add(value);
    }

    public string GetString(int argIndex)
    {
        return _argsByIndex[argIndex].GetAsString();
    }

    public string GetString(string argName)
    {
        return _argsByName[argName].GetAsString();
    }
    
}