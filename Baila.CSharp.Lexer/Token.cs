namespace Baila.CSharp.Lexer;

public record struct Token(TokenType Type, string Value = "");