using System.Text;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Typing;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public record StringConcatExpression(
    IEnumerable<string> FixedStrings,
    IEnumerable<IExpression> Expressions,
    string Filename,
    SyntaxNodeSpan Span) : IExpression
{
    public BailaType GetBailaType() => BailaType.String;

    public IValue Evaluate()
    {
        var sb = new StringBuilder();

        for (int i = 0;; i++)
        {
            var fixedString = FixedStrings.Skip(i).FirstOrDefault();
            if (fixedString == null) break;

            sb.Append(fixedString);

            var expression = Expressions.Skip(i).FirstOrDefault();
            if (expression == null) break;

            sb.Append(expression.Evaluate().GetAsString());
        }
        
        return new StringValue(sb.ToString());
    }

    public void AcceptVisitor(VisitorBase visitor)
    {
        visitor.VisitStringConcatExpression(this);
    }

    public string Stringify()
    {
        return "TODO";
    }

    public override string ToString()
    {
        return $"StringConcatExpression(TODO)";
    }
}