using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Statements;

public record NoOpStatement : IStatement
{
    public void Execute()
    {
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitNoOpStatement(this);
    }
}