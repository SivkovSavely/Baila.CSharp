using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Statements;

public interface IStatement
{
    void Execute();

    void AcceptVisitor(VisitorBase visitor);
}