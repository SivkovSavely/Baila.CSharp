using Baila.CSharp.Ast.Syntax;

namespace Baila.CSharp.Lexer;

public record Token : ISyntaxNode
{
    public Token(string filename, SyntaxNodeSpan span, TokenType type, string? value = null)
    {
        Type = type;
        Value = value;
        Filename = filename;
        Span = span;

        if (type.HasMeaningfulValue && value is null)
        {
            throw new Exception($"TokenType {type} has meaningful value, but value provided was null.");
        }
    }

    public override string ToString()
    {
        return Type.HasMeaningfulValue ? Value! : Type.Type;
    }

    public TokenType Type { get; }
    public string? Value { get; }
    public string Filename { get; init; }
    public SyntaxNodeSpan Span { get; init; }
}