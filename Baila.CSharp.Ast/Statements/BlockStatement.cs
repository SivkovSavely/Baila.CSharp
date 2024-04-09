﻿namespace Baila.CSharp.Ast.Statements;

public class BlockStatement : IStatement
{
    private readonly List<IStatement> _statements = [];

    public IEnumerable<IStatement> Statements => _statements.AsReadOnly();

    public void AddStatement(IStatement statement)
    {
        _statements.Add(statement);
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