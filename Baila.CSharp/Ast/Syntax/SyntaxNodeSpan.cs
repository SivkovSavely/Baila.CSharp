namespace Baila.CSharp.Ast.Syntax;

public readonly record struct SyntaxNodeSpan(int StartLine, int StartColumn, int LineCount, int SyntaxLength)
{
    /// <summary>
    /// <code>
    /// line OurSyntaxNode line ← Line
    /// 00000000011111111112222 ← Column tens
    /// 12345678901234567890123 ← Column units
    ///      ^^^^^^^^^^^^^      ← Our syntax node
    ///      ^             = 6  ← StartColumn
    ///      1234567890123 = 13 ← SyntaxLength
    ///                  ^ = 18 ← EndColumn
    /// EndColumn == 18 == 13 + 6 - 1
    /// </code>
    /// </summary>
    /// <remarks>
    /// If the syntax node in question consists of one character,
    /// EndColumn will be equal to StartColumn
    /// </remarks>
    public int EndColumn => StartColumn + SyntaxLength - 1;
    public int EndLine => StartLine + LineCount - 1;

    public static SyntaxNodeSpan Empty => new(1, 1, 1, 1);
}