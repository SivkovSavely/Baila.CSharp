namespace Baila.CSharp.Lexer;

public record Token
{
    public Token(Cursor cursor, TokenType type, string? value = null)
    {
        Cursor = cursor;
        Type = type;
        Value = value;

        if (type.HasMeaningfulValue && value is null)
        {
            throw new Exception($"TokenType {type} has meaningful value, but value provided was null.");
        }
    }

    public override string ToString()
    {
        return Type.HasMeaningfulValue ? Value! : Type.Type;
    }

    public Cursor Cursor { get; }
    public TokenType Type { get; }
    public string? Value { get; }

    public void Deconstruct(out Cursor cursor, out TokenType type, out string? value)
    {
        cursor = Cursor;
        type = Type;
        value = Value;
    }
}