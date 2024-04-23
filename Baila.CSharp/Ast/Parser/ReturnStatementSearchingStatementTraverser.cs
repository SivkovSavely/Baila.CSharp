using Baila.CSharp.Ast.Statements;

namespace Baila.CSharp.Parser;

public static class ReturnStatementSearchingStatementTraverser
{
    public static List<ReturnStatement> Search(IStatement statement)
    {
        var list = new List<ReturnStatement>();

        switch (statement)
        {
            case BlockStatement blockStatement:
                foreach (var stmt in blockStatement.Statements)
                {
                    list.AddRange(Search(stmt));
                }
                break;
            case ConstantDefineStatement constantDefineStatement:
                break;
            case DoWhileStatement doWhileStatement:
                list.AddRange(Search(doWhileStatement.Body));
                break;
            case ExpressionStatement expressionStatement:
                break;
            case ForStatement forStatement:
                list.AddRange(Search(forStatement.Body));
                break;
            case FunctionDefineStatement functionDefineStatement:
                throw new Exception("Unsupported function inside another function");
                break;
            case IfElseStatement ifElseStatement:
                list.AddRange(Search(ifElseStatement.TrueStatement));
                if (ifElseStatement.FalseStatement != null)
                    list.AddRange(Search(ifElseStatement.FalseStatement));
                break;
            case NoOpStatement noOpStatement:
                break;
            case ReturnStatement returnStatement:
                list.Add(returnStatement);
                break;
            case VariableDefineStatement variableDefineStatement:
                break;
            case WhileStatement whileStatement:
                list.AddRange(Search(whileStatement.Body));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(statement));
        }

        return list;
    }
}