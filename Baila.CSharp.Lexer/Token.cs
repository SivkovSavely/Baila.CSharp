namespace Baila.CSharp.Lexer;

public readonly record struct Token(Cursor Cursor, TokenType Type, string? Value = null)
{
    public override string ToString()
    {
        return string.IsNullOrEmpty(Value)
            ? $"Token {{ Type = {Type.Type} }}"
            : $"Token {{ Type = {Type.Type}, Value = {Value} }}";
    }
}