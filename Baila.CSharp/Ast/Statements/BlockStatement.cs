using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Statements;

public class BlockStatement(List<IStatement>? statements = null) : IStatement
{
    private readonly List<IStatement> _statements = statements ?? [];

    public IEnumerable<IStatement> Statements => _statements.AsReadOnly();

    public void AddStatement(IStatement statement)
    {
        _statements.Add(statement);
    }

    public void Execute()
    {
        NameTable.PushScope();

        foreach (var stmt in _statements)
        {
            stmt.Execute();
        }
        
        NameTable.PopScope();
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitBlockStatement(this);
    }

    public override string ToString()
    {
        if (_statements.Count == 0)
        {
            return $"BlockStatement(Count={_statements.Count}) {{}}";
        }
        return $"BlockStatement(Count={_statements.Count}) {{\n" +
               string.Join("\n", _statements) +
               $"\n}}";
    }
}