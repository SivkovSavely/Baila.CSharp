﻿using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Statements;

public record BlockStatement(
    string Filename,
    SyntaxNodeSpan Span) : IStatement
{
    private readonly List<IStatement> _statements = [];

    public BlockStatement(
        List<IStatement> statementList,
        string Filename,
        SyntaxNodeSpan Span) : this(Filename, Span)
    {
        _statements = statementList;
    }

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