namespace Baila.CSharp.Lexer;

public record struct Token(Cursor Cursor, TokenType Type, string Value = "");