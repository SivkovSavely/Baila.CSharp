namespace Baila.CSharp.Ast.Diagnostics;

public class ParseException : Exception
{
    public IEnumerable<IDiagnostic> Diagnostics { get; }

    public ParseException(IEnumerable<IDiagnostic> diagnostics)
    {
        Diagnostics = diagnostics;
    }
}