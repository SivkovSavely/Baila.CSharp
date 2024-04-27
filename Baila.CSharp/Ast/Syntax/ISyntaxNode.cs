namespace Baila.CSharp.Ast.Syntax;

public interface ISyntaxNode
{
    public SyntaxNodeSpan Span { get; init; }
}