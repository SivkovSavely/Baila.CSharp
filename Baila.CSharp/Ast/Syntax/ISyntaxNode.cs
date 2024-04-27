namespace Baila.CSharp.Ast.Syntax;

public interface ISyntaxNode
{
    public string Filename { get; init; }

    public SyntaxNodeSpan Span { get; init; }
}