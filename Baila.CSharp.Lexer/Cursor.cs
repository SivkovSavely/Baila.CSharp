namespace Baila.CSharp.Lexer;

public class Cursor(int position, int column, int line, string filename) : ICloneable
{
    public int Position { get; set; } = position;
    public int Column { get; set; } = column;
    public int Line { get; set; } = line;
    public string Filename { get; set; } = filename;

    public Cursor Clone()
    {
        return new Cursor(position, column, line, filename);
    }
    object ICloneable.Clone()
    {
        return Clone();
    }
}