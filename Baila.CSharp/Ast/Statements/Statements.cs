using Baila.CSharp.Runtime.Values.Abstractions;

namespace Baila.CSharp.Ast.Statements;

public class Statements : IStatement
{
    private readonly List<IStatement> _statements = [];

    public IEnumerable<IStatement> StatementList => _statements.AsReadOnly();

    public IValue? LastEvaluatedValue { get; private set; }

    public void AddStatement(IStatement statement)
    {
        _statements.Add(statement);
    }

    public void Execute()
    {
        foreach (var stmt in _statements)
        {
            if (stmt is ExpressionStatement expressionStatement)
            {
                expressionStatement.Execute();
                LastEvaluatedValue = expressionStatement.Value;
            }
            else
            {
                stmt.Execute();
            }
        }
    }

    public override string ToString()
    {
        if (_statements.Count == 0)
        {
            return $"Statements(Count={_statements.Count}) {{}}";
        }
        return $"Statements(Count={_statements.Count}) {{\n" +
               string.Join("\n", _statements) +
               $"\n}}";
    }
}