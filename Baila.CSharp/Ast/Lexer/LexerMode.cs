namespace Baila.CSharp.Ast.Lexer;

public enum LexerMode
{
    /// <summary>
    /// Regular lexer mode for interpreting/compiling.
    /// </summary>
    Regular,
    /// <summary>
    /// Lexer mode for the inside of an interpolated string literal template.
    /// </summary>
    InterpolatedString,
    /// <summary>
    /// Lexer mode for the syntax highlighter. Includes whitespace and comments.
    /// </summary>
    Highlighting,
    /// <summary>
    /// Lexer mode for the insides of an interpolated string literal template for the syntax highlighter.
    /// </summary>
    HighlightingInterpolatedString,
}