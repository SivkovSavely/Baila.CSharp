using System.Diagnostics.CodeAnalysis;
using System.Text;
using Baila.CSharp.Ast.Lexer;
using Baila.CSharp.Runtime.Types;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public record StringConcatExpression(
    Token StartToken,
    IEnumerable<string> FixedStrings,
    IEnumerable<IExpression> Expressions,
    Token EndToken) : IExpression
{
    public SyntaxNodeSpan Span { get; init; } = SyntaxNodeSpan.Merge(StartToken, EndToken);
    
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

    [ExcludeFromCodeCoverage]
    public string Stringify()
    {
        return "TODO";
    }

    [ExcludeFromCodeCoverage]
    public override string ToString()
    {
        return $"StringConcatExpression(TODO)";
    }
}