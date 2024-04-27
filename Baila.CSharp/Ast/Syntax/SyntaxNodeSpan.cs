namespace Baila.CSharp.Ast.Syntax;

public readonly record struct SyntaxNodeSpan(
    string Filename, int StartLine, int StartColumn, int LineCount, int SyntaxLength)
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

    public static SyntaxNodeSpan Empty => new("", 1, 1, 1, 1);
    public static SyntaxNodeSpan ThrowEmpty => throw new Exception("SyntaxNodeSpan.ThrowEmpty");

    public SyntaxNodeSpan WithExpandedToTheLeft(int leftColumnExpansion) =>
        this with { StartColumn = StartColumn - leftColumnExpansion, SyntaxLength = SyntaxLength + leftColumnExpansion };

    public SyntaxNodeSpan WithExpandedToTheRight(int rightColumnExpansion) =>
        this with { SyntaxLength = SyntaxLength + rightColumnExpansion };

    public static SyntaxNodeSpan FromEnd(string filename, int startLine, int startColumn, int endLine, int endColumn)
    {
        if (endLine < startLine) throw new Exception("Span end line cannot be before span start line");
        if (startLine == endLine && endColumn < startColumn) throw new Exception("Span end column cannot be before span start column");
        return new SyntaxNodeSpan(
            filename, startLine, startColumn, endLine - startLine + 1, endColumn - startColumn + 1);
    }

    public static SyntaxNodeSpan Merge(params SyntaxNodeSpan[] spans)
    {
        return Merge(false, spans);
    }

    private static SyntaxNodeSpan Merge(bool ignoreMinLength, params SyntaxNodeSpan[] spans)
    {
        if (!ignoreMinLength && spans.Length < 2)
        {
            throw new ArgumentException("Span array should have at least two elements", nameof(spans));
        }
        
        var firstSpan = spans.OrderBy(s => s.StartLine).ThenBy(s => s.StartColumn).First();
        var lastSpan = spans.OrderByDescending(s => s.EndLine).ThenByDescending(s => s.EndColumn).First();
        return FromEnd(firstSpan.Filename, firstSpan.StartLine, firstSpan.StartColumn, lastSpan.EndLine, lastSpan.EndColumn);
    }

    public static SyntaxNodeSpan Merge(params ISyntaxNode[] nodes)
    {
        return Merge(nodes.Select(n => n.Span).ToArray());
    }

    public static SyntaxNodeSpan MergeWithNulls(params ISyntaxNode?[] nodes)
    {
        return Merge(true, nodes.Where(n => n != null).Select(n => n!.Span).ToArray());
    }
}