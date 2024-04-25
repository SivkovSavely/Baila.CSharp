using Baila.CSharp.Ast.Expressions;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Statements;

public record ConstantDefineStatement(string Name, IExpression Value) : IStatement
{
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