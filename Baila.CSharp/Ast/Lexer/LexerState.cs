namespace Baila.CSharp.Ast.Lexer;

public enum LexerState
{
    /// <summary>
    /// Parse value like number, string, regex, etc
    /// </summary>
    Value,
    /// <summary>
    /// Parse operator (excluding parentheses) like +, -, *, /, etc
    /// </summary>
    Operator,
}