using Baila.CSharp.Lexer;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public record IntValueExpression(
    long Value,
    Token SourceToken) : ValueExpression(SourceToken)
{
    public override BailaType GetBailaType() => BailaType.Int;

    public override IValue Evaluate()
    {
        return new IntValue(Value);
    }

    public override void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitIntValueExpression(this);
    }

    public override string Stringify()
    {
        return Value.ToString();
    }

    public override string ToString()
    {
        return $"IntValueExpression({Value})";
    }
}