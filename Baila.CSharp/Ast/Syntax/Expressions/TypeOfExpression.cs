using System.Diagnostics.CodeAnalysis;
using Baila.CSharp.Ast.Lexer;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Types;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public record TypeOfExpression(
    Token TypeOfKeyword,
    IExpression Expression) : IExpression
{
    public SyntaxNodeSpan Span { get; init; } = SyntaxNodeSpan.Merge(TypeOfKeyword, Expression);

    public BailaType? GetBailaType()
    {
        return Expression.GetBailaType();
    }

    public IValue Evaluate()
    {
        if (Expression is VariableExpression variableExpression)
        {
            return new StringValue(NameTable.Get(variableExpression.Name).Type.ToString());
        }

        return new StringValue(Expression.GetBailaType()!.ToString());
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitTypeOfExpression(this);
    }

    [ExcludeFromCodeCoverage]
    public string Stringify()
    {
        return Expression.Stringify();
    }

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        return $"TypeOfExpression({Expression})";
    }
}