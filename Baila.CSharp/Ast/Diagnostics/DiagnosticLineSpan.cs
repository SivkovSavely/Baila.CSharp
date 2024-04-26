namespace Baila.CSharp.Ast.Diagnostics;

public record DiagnosticLineSpan(string FullLine, int LineNumber, int StartColumn, int Length);