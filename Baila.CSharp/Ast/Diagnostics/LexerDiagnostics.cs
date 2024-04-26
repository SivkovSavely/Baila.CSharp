using Baila.CSharp.Lexer;

namespace Baila.CSharp.Ast.Diagnostics;

public class LexerDiagnostics
{
    public static LexerDiagnostic BL0001_SecondDecimalPoint(Cursor cursor, string line) =>
        new(
            "encountered second decimal point in a number literal",
            cursor,
            line);

    public static LexerDiagnostic BL0002_EmptyInterpolatedStringExpression(Cursor cursor, string line) =>
        new(
            "empty interpolated string expression",
            cursor,
            line);

    public static LexerDiagnostic BL0003_UnexpectedStringInterpolationExpressionClosingBrace(Cursor cursor, string line) =>
        new(
            "unexpected interpolated string expression closing brace",
            cursor,
            line);

    public static LexerDiagnostic BL0004_UnrecognizedStringEscapeSequence(char escapeSequenceChar, Cursor cursor, string line) =>
        new(
            $"unrecognized escape sequence: {escapeSequenceChar}",
            cursor,
            line);

    public static LexerDiagnostic BL0005_UnbalancedBracesInsideTemplateString(Cursor cursor, string line) =>
        new(
            "unbalanced braces inside template string",
            cursor,
            line);

    public static LexerDiagnostic BL0006_UnclosedString(Cursor cursor, string line) =>
        new(
            "unclosed string literal",
            cursor,
            line);
}

public class LexerDiagnostic : IDiagnostic
{
    public string Message { get; }
    public Cursor Cursor { get; }
    public string Line { get; }

    internal LexerDiagnostic(string message, Cursor cursor, string line)
    {
        Message = message;
        Cursor = cursor;
        Line = line;
    }

    public string GetErrorMessage()
    {
        return Message;
    }

    public string GetFilename()
    {
        return Cursor.Filename;
    }

    public IEnumerable<DiagnosticLineSpan> GetLines()
    {
        return [
            new DiagnosticLineSpan(Line, Cursor.Line, Cursor.Column, 1)
        ];
    }

    public bool ShouldShowCode()
    {
        return true;
    }
}