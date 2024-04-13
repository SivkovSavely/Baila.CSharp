namespace Baila.CSharp.Lexer;

public record Token(Cursor Cursor, TokenType Type, string? Value = null)
{
    public override string ToString()
    {
        return Type == TokenType.Identifier ? Value! : Type.Type;
    }
}