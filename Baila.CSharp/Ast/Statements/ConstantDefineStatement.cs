using Baila.CSharp.Ast.Expressions;

namespace Baila.CSharp.Ast.Statements;

public class ConstantDefineStatement(string name, IExpression value) : IStatement
{
    public string Name { get; } = name;
    public IExpression Value { get; } = value;

    public override string ToString()
    {
        return $"ConstantDefineStatement(Name={Name}, " +
               $"Value={Value})";
    }
}