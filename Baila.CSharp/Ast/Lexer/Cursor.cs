namespace Baila.CSharp.Ast.Lexer;

public class Cursor(int position, int column, int line, string filename) : ICloneable
{
    public int Position { get; set; } = position;
    public int Column { get; set; } = column;
    public int Line { get; set; } = line;
    public string Filename { get; set; } = filename;

    public Cursor Clone()
    {
        return new Cursor(Position, Column, Line, Filename);
    }
    object ICloneable.Clone()
    {
        return Clone();
    }

    public override string ToString()
    {
        return $"{Path.GetFileName(Filename)}:{Line}:{Column}";
    }
}