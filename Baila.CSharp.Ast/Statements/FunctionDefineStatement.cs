using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Typing;

namespace Baila.CSharp.Ast.Statements;

public class FunctionDefineStatement(
    string name,
    List<FunctionDefineStatement.FunctionParameter> parameters,
    IStatement body,
    BailaType? returnType) : IStatement
{
    public string Name { get; } = name;
    public List<FunctionParameter> Parameters { get; } = parameters;
    public IStatement Body { get; } = body;
    public BailaType? ReturnType { get; } = returnType;
    
    public record FunctionParameter(string Name, BailaType Type, IExpression? DefaultValue, bool Vararg);

    public override string ToString()
    {
        return $"FunctionDefineStatement(Name={Name})";
    }
}