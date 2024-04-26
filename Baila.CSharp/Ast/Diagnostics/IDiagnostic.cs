namespace Baila.CSharp.Ast.Diagnostics;

public interface IDiagnostic
{
    string GetErrorMessage();
    string GetFilename();
    IEnumerable<DiagnosticLineSpan> GetLines();
    bool ShouldShowCode();
}