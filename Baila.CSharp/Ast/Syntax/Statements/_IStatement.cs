using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Statements;

public interface IStatement : ISyntaxNode
{
    void Execute();

    void AcceptVisitor(VisitorBase visitor);
}