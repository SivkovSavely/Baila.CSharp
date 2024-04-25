using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Statements;

public class NoOpStatement : IStatement
{
    public void Execute()
    {
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitNoOpStatement(this);
    }
}