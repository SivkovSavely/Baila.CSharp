namespace Baila.CSharp.Lexer;

public class Cursor(int position, int column, int line, string filename)
{
    public int Position { get; set; } = position;
    public int Column { get; set; } = column;
    public int Line { get; set; } = line;
    public string Filename { get; set; } = filename;
}