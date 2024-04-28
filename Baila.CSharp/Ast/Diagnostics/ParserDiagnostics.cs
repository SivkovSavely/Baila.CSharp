using Baila.CSharp.Ast.Syntax;
using Baila.CSharp.Lexer;

namespace Baila.CSharp.Ast.Diagnostics;

public class ParserDiagnostics
{
    public static ParserDiagnostic BP0000_Test(ISyntaxNode node, string[] relevantSourceLines) =>
        new(
            "BP0000",
            $"test diagnostic for {node.GetType().Name}",
            node,
            relevantSourceLines);
    public static ParserDiagnostic BP0001_UnexpectedToken(Token unexpectedToken, TokenType[] expectedTokenTypes, string[] relevantSourceLines) =>
        new(
            "BP0001",
            $"unexpected token '{unexpectedToken}', expected tokens: {string.Join(", ", expectedTokenTypes.Select(x => $"'{x}'"))}",
            unexpectedToken,
            relevantSourceLines);

    public static ParserDiagnostic BP0001_UnexpectedToken(Token unexpectedToken, string expected, string[] relevantSourceLines) =>
        new(
            "BP0001",
            $"unexpected token '{unexpectedToken}', expected {expected}",
            unexpectedToken,
            relevantSourceLines);
    public static ParserDiagnostic BP0002_ExpectedToInForLoop(Token unexpectedToken, string[] relevantSourceLines) =>
        new(
            "BP0002",
            "'to' expected in 'for' loop. For C-like style of 'for' loop, please use 'while' instead",
            unexpectedToken,
            relevantSourceLines);
    public static ParserDiagnostic BP0003_CountNotInferTypeOfNumber(Token malformedNumber, string[] relevantSourceLines) =>
        new(
            "BP0003",
            "could not infer type of the number",
            malformedNumber,
            relevantSourceLines);
}

public class ParserDiagnostic : IDiagnostic
{
    private readonly string[] _relevantSourceLines;
    public string Code { get; }
    public string Message { get; }
    public ISyntaxNode SyntaxNode { get; }

    internal ParserDiagnostic(string code, string message, ISyntaxNode syntaxNode, string[] relevantSourceLines)
    {
        _relevantSourceLines = relevantSourceLines;
        Code = code;
        Message = message;
        SyntaxNode = syntaxNode;
    }

    public string GetCode()
    {
        return Code;
    }

    public string GetErrorMessage()
    {
        return Message;
    }

    public string GetFilename()
    {
        return SyntaxNode.Span.Filename;
    }

    public IEnumerable<DiagnosticLineSpan> GetLines()
    {
        for (int lineNumber = 0; lineNumber < SyntaxNode.Span.LineCount; lineNumber++)
        {
            var fullLine = _relevantSourceLines[lineNumber];
            int startColumn, length;
            
            if (SyntaxNode.Span.LineCount == 1) {
                startColumn = SyntaxNode.Span.StartColumn;
                length = SyntaxNode.Span.SyntaxLength;
            } else {
                if (lineNumber + 1 == SyntaxNode.Span.StartLine) {
                    startColumn = SyntaxNode.Span.StartColumn;
                    length = fullLine.Length - startColumn;
                } else if (lineNumber + 1 == SyntaxNode.Span.EndLine) {
                    startColumn = 1;
                    length = SyntaxNode.Span.EndColumn - 1;
                } else {
                    startColumn = 1;
                    length = fullLine.Length;
                }
            }
            
            yield return new DiagnosticLineSpan(
                fullLine,
                SyntaxNode.Span.StartLine + lineNumber,
                startColumn,
                length);
        }
    }

    public bool ShouldShowCode()
    {
        return true;
    }
}