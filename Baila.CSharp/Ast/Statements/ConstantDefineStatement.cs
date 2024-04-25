using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Statements;

public class ConstantDefineStatement(string name, IExpression value) : IStatement
{
    public string Name { get; } = name;
    public IExpression Value { get; } = value;

    public void Execute()
    {
        NameTable.AddConstant(Name, Value.Evaluate());
    }


    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitConstantDefineStatement(this);
    }

    public override string ToString()
    {
        return $"ConstantDefineStatement(Name={Name}, " +
               $"Value={Value})";
    }
}