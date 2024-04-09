using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Typing;

namespace Baila.CSharp.Ast.Statements;

public class VariableDefineStatement(string name, BailaType? type, IExpression? value) : IStatement
{
    public string Name { get; } = name;
    public BailaType? Type { get; } = type;
    public IExpression? Value { get; } = value;

    public override string ToString()
    {
        return $"VariableDefineStatement(Name={Name}, " +
               $"Type={Type}, " +
               $"Value={Value})";
    }
}