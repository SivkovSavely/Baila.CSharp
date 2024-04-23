using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;

namespace Baila.CSharp.Ast.Expressions;

public class VariableExpression(string name) : IExpression
{
    public string Name { get; } = name;

    public string Stringify()
    {
        return Name;
    }

    public BailaType GetBailaType()
    {
        var variable = NameTable.Get(Name);
        return variable.Type;
    }

    public IValue Evaluate()
    {
        var member = NameTable.Get(Name);
        return member.Value;
    }

    public override string ToString()
    {
        return $"VariableExpression({Name})";
    }
}