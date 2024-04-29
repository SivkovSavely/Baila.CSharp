using System.Globalization;
using Baila.CSharp.Lexer;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public record FloatValueExpression(
    double Value,
    Token SourceLiteral) : ValueExpression(SourceLiteral)
{
    public override BailaType GetBailaType() => BailaType.Float;

    public override IValue Evaluate()
    {
        return new FloatValue(Value);
    }

    public override void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitFloatValueExpression(this);
    }

    public override string Stringify()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }

    public override string ToString()
    {
        return $"FloatValueExpression({Value})";
    }
}