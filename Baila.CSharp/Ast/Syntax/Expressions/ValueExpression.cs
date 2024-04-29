using Baila.CSharp.Lexer;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public abstract record ValueExpression(Token SourceLiteral) : IExpression
{
    public virtual SyntaxNodeSpan Span { get; init; } = SourceLiteral.Span;
    
    public abstract BailaType GetBailaType();

    public abstract IValue Evaluate();

    public abstract void AcceptVisitor(VisitorBase visitor);

    public abstract string Stringify();
}