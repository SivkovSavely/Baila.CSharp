using Baila.CSharp.Ast.Lexer;
using Baila.CSharp.Ast.Syntax;

namespace Baila.CSharp.Ast.Diagnostics;

public class LexerDiagnostics
{
    public static LexerDiagnostic BL0001_SecondDecimalPoint(Cursor cursor, string line) =>
        new(
            "BL0001",
            "encountered second decimal point in a number literal",
            cursor,
            line);

    public static LexerDiagnostic BL0002_EmptyInterpolatedStringExpression(Cursor cursor, string line) =>
        new(
            "BL0002",
            "empty interpolated string expression",
            cursor,
            line);

    public static LexerDiagnostic BL0003_UnexpectedStringInterpolationExpressionClosingBrace(Cursor cursor, string line) =>
        new(
            "BL0003",
            "unexpected interpolated string expression closing brace",
            cursor,
            line);

    public static LexerDiagnostic BL0004_UnrecognizedStringEscapeSequence(char escapeSequenceChar, Cursor cursor, string line) =>
        new(
            "BL0004",
            $"unrecognized escape sequence: {escapeSequenceChar}",
            cursor,
            line);

    public static LexerDiagnostic BL0005_UnbalancedBracesInsideTemplateString(Cursor cursor, string line) =>
        new(
            "BL0005",
            "unbalanced braces inside template string",
            cursor,
            line);

    public static LexerDiagnostic BL0006_UnclosedString(SyntaxNodeSpan span, string line) =>
        new(
            "BL0006",
            "unclosed string literal",
            span,
            line);
}

public class LexerDiagnostic : IDiagnostic
{
    public string Code { get; }
    public string Message { get; }
    public SyntaxNodeSpan Span { get; }
    public string Line { get; }

    internal LexerDiagnostic(string code, string message, Cursor cursor, string line)
    {
        Code = code;
        Message = message;
        Span = new SyntaxNodeSpan(cursor.Filename, cursor.Line, cursor.Column, 1, 1);
        Line = line;
    }

    internal LexerDiagnostic(string code, string message, SyntaxNodeSpan span, string line)
    {
        Code = code;
        Message = message;
        Span = span;
        Line = line;
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
        return Span.Filename;
    }

    public IEnumerable<DiagnosticLineSpan> GetLines()
    {
        return [
            new DiagnosticLineSpan(Line, Span.StartLine, Span.StartColumn, Span.SyntaxLength)
        ];
    }

    public bool ShouldShowCode()
    {
        return true;
    }
}