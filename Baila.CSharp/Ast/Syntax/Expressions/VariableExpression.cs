using System.Diagnostics.CodeAnalysis;
using Baila.CSharp.Ast.Lexer;
using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Runtime.Types;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public record VariableExpression(
    string Name,
    Token IdentifierToken) : IExpression
{
    public SyntaxNodeSpan Span { get; init; } = IdentifierToken.Span;

    [ExcludeFromCodeCoverage]
    public string Stringify()
    {
        return Name;
    }

    public BailaType GetBailaType()
    {
        var function = CompileTimeNameTable.GetFunction(Name);
        var variable = CompileTimeNameTable.Get(Name);
        return function != null ? BailaType.Function : variable?.Type!; // TODO somehow resolve the method group with some functional type
    }

    public IValue Evaluate()
    {
        var member = NameTable.Get(Name);
        return member.Value;
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitVariableExpression(this);
    }

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        return $"VariableExpression({Name})";
    }
}