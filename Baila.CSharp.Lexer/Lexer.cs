using System.Security.Principal;
using System.Text;

namespace Baila.CSharp.Lexer;

public class Lexer(string source, string filename)
{
    private readonly Cursor _cursor = new(0, 1, 1, filename);
    private readonly List<Token> _tokens = [];
    private readonly StringBuilder _buffer = new(256);

    private readonly char[] _operatorChars = "+-*/=".ToCharArray();

    private readonly Dictionary<string, TokenType> _operatorMap = new()
    {
        ["+"] = TokenType.Plus,
        ["-"] = TokenType.Minus,
        ["*"] = TokenType.Star,
        ["/"] = TokenType.Slash,
        ["="] = TokenType.Eq,
    };

    private bool HasChars() => _cursor.Position < source.Length;

    public List<Token> Tokenize()
    {
        while (HasChars())
        {
            var current = Current();

            if (current is >= '0' and <= '9')
            {
                TokenizeNumber();
            }
            else if (IsIdentifierStart(current))
            {
                TokenizeIdentifier();
            }
            else if (_operatorChars.Contains(current))
            {
                TokenizeOperator();
            }
            else
            {
                Next(); // ignore everything else
            }
        }

        return _tokens;
    }

    private void TokenizeOperator()
    {
        _buffer.Clear();

        var current = Current();
        while (HasChars())
        {
            if (!_operatorMap.ContainsKey(_buffer.ToString() + current))
            {
                break;
            }

            _buffer.Append(current);
            current = Next();
        }

        var op = _buffer.ToString();
        _tokens.Add(new Token(_operatorMap[op], op));
    }

    private void TokenizeIdentifier()
    {
        _buffer.Clear();

        var current = Current();
        while (HasChars())
        {
            if (!IsIdentifierContinuation(current))
            {
                break;
            }

            _buffer.Append(current);
            current = Next();
        }

        _tokens.Add(new Token(TokenType.Identifier, _buffer.ToString()));
    }

    private void TokenizeNumber()
    {
        _buffer.Clear();

        var current = Current();
        while (HasChars())
        {
            if (current is not (>= '0' and <= '9') and not '_' and not '.')
            {
                break;
            }

            _buffer.Append(current);
            current = Next();
        }

        _tokens.Add(new Token(TokenType.Number, _buffer.ToString()));
    }

    private bool IsIdentifierStart(char ch)
    {
        return ch is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_' or '$';
    }

    private bool IsIdentifierContinuation(char ch)
    {
        return ch is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '_' or '$';
    }

    private char Next()
    {
        _cursor.Position++;

        if (Current() == '\n')
        {
            _cursor.Line++;
            _cursor.Column = 1;
            _cursor.Position++; // skip newline
        }

        return Current();
    }

    private char Current()
    {
        var pos = _cursor.Position;
        return pos < source.Length ? source[pos] : '\0';
    }
}