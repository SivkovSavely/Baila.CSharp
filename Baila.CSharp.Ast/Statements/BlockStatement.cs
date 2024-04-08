namespace Baila.CSharp.Ast.Statements;

public class BlockStatement : IStatement
{
    private readonly List<IStatement> _statements = [];

    public IEnumerable<IStatement> Statements => _statements.AsReadOnly();

    public void AddStatement(IStatement statement)
    {
        _statements.Add(statement);
    }
    
    public void Execute()
    {
        foreach (var statement in _statements)
        {
            statement.Execute();
        }
    }

    public override string ToString()
    {
        return $"Statements({_statements.Count}) {{\n" +
               string.Join("\n", _statements) +
               $"\n}}";
    }
}