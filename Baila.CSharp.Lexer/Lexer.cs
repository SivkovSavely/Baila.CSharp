using System.Text;

namespace Baila.CSharp.Lexer;

public class Lexer(string source, string filename)
{
    private readonly Cursor _cursor = new(0, 1, 1, filename);
    private readonly List<Token> _tokens = [];
    private readonly StringBuilder _buffer = new(256);

    private readonly char[] _operatorChars = "+-*/%=!~&|^~?:;<>.,".ToCharArray();
    private readonly char[] _valueOperatorChars = "+-*!~?<".ToCharArray();
    private readonly char[] _bracketChars = "()[]{}".ToCharArray();

    private readonly Dictionary<string, TokenType> _operatorMap = new()
    {
        ["+"] = TokenType.Plus,
        ["+="] = TokenType.PlusEq,
        ["++"] = TokenType.PlusPlus,
        ["-"] = TokenType.Minus,
        ["-="] = TokenType.MinusEq,
        ["--"] = TokenType.MinusMinus,
        ["*"] = TokenType.Star,
        ["**"] = TokenType.StarStar,
        ["*="] = TokenType.StarEq,
        ["**="] = TokenType.StarStarEq,
        ["/"] = TokenType.Slash,
        ["//"] = TokenType.SlashSlash,
        ["/="] = TokenType.SlashEq,
        ["//="] = TokenType.SlashSlashEq,
        ["%"] = TokenType.Percent,
        ["%="] = TokenType.PercentEq,
        ["="] = TokenType.Eq,
        ["=="] = TokenType.EqEq,
        ["==="] = TokenType.EqEqEq,
        ["!="] = TokenType.ExclEq,
        ["!=="] = TokenType.ExclEqEq,
        ["|"] = TokenType.Bar,
        ["||"] = TokenType.BarBar,
        ["|="] = TokenType.BarEq,
        ["||="] = TokenType.BarBarEq,
        ["|>"] = TokenType.Pipe,
        ["&"] = TokenType.Amp,
        ["&&"] = TokenType.AmpAmp,
        ["&="] = TokenType.AmpEq,
        ["&&="] = TokenType.AmpAmpEq,
        ["~"] = TokenType.Tilde,
        ["^"] = TokenType.Caret,
        ["^="] = TokenType.CaretEq,
        ["^^"] = TokenType.CaretCaret,
        ["^^="] = TokenType.CaretCaretEq,
        ["."] = TokenType.Dot,
        [".."] = TokenType.DotDot,
        [","] = TokenType.Comma,
        ["?."] = TokenType.NullDot,
        ["??"] = TokenType.Elvis,
        ["??="] = TokenType.ElvisEq,
        ["<"] = TokenType.Lt,
        ["<="] = TokenType.LtEq,
        ["<<"] = TokenType.LtLt,
        ["<<="] = TokenType.LtLtEq,
        [">"] = TokenType.Gt,
        [">="] = TokenType.GtEq,
        [">>"] = TokenType.GtGt,
        [">>="] = TokenType.GtGtEq,
        [">>>"] = TokenType.GtGtGt,
        [">>>="] = TokenType.GtGtGtEq,
        ["->"] = TokenType.SingleArrow,
        ["=>"] = TokenType.DoubleArrow,
        ["("] = TokenType.LeftParen,
        [")"] = TokenType.RightParen,
        ["["] = TokenType.LeftBracket,
        ["]"] = TokenType.RightBracket,
        ["{"] = TokenType.LeftCurly,
        ["}"] = TokenType.RightCurly,
        ["!"] = TokenType.Excl,
        ["?"] = TokenType.Question,
        [":"] = TokenType.Colon,
        ["::"] = TokenType.ColonColon,
        [";"] = TokenType.Semicolon,
    };

    private readonly Dictionary<string, TokenType> _keywordMap = new()
    {
        ["null"] = TokenType.Null,
        ["true"] = TokenType.True,
        ["false"] = TokenType.False,
        ["this"] = TokenType.This,
        ["super"] = TokenType.Super,

        ["var"] = TokenType.Var,
        ["const"] = TokenType.Const,
        ["prop"] = TokenType.Property,
        ["property"] = TokenType.Property,
        ["func"] = TokenType.Function,
        ["function"] = TokenType.Function,
        ["class"] = TokenType.Class,
        ["struct"] = TokenType.Struct,
        ["interface"] = TokenType.Interface,
        ["enum"] = TokenType.Enum,
        ["operator"] = TokenType.Operator,

        ["constructor"] = TokenType.Constructor,
        ["deconstructor"] = TokenType.Deconstructor,

        ["typeof"] = TokenType.Typeof,
        ["from"] = TokenType.From,
        ["import"] = TokenType.Import,
        ["export"] = TokenType.Export,

        ["ref"] = TokenType.Ref,

        ["if"] = TokenType.If,
        ["else"] = TokenType.Else,
        ["switch"] = TokenType.Switch,
        ["for"] = TokenType.For,
        ["do"] = TokenType.Do,
        ["while"] = TokenType.While,
        ["try"] = TokenType.Try,
        ["catch"] = TokenType.Catch,
        ["finally"] = TokenType.Finally,

        ["global"] = TokenType.Global,
        ["public"] = TokenType.Public,
        ["private"] = TokenType.Private,
        ["protected"] = TokenType.Protected,
        ["override"] = TokenType.Override,
        ["sealed"] = TokenType.Sealed,
        ["static"] = TokenType.Static,

        ["async"] = TokenType.Async,
        ["await"] = TokenType.Await,

        // Keywords that are actually operators by logic
        ["break"] = TokenType.Break,
        ["continue"] = TokenType.Continue,
        ["throw"] = TokenType.Throw,
        ["return"] = TokenType.Return,
        ["yield"] = TokenType.Yield,
        ["in"] = TokenType.In,
        ["is"] = TokenType.Is,
        ["as"] = TokenType.As,
    };

    private bool HasChars() => _cursor.Position < source.Length;

    public List<Token> Tokenize()
    {
        var state = LexerState.Value;
        var parenthesisParity = 0;
        var bracketParity = 0;

        // Checking for ParseState.Value implies that
        //   you are tokenizing either value or something that should precede a value
        // Checking for ParseState.Operator implies that
        //   you are tokenizing something that should be between values.
        // Most of the time state should switch.

        while (HasChars())
        {
            var currentChar = Current();
            var nextChar = Peek(1);

            if (currentChar != '0' && IsDigit(currentChar) || currentChar == '.' && IsDigit(nextChar))
            {
                TokenizeDecimalNumber();
                state = LexerState.Operator;
            }
            else if (IsDigit(currentChar))
            {
                switch (nextChar)
                {
                    case 'b' or 'B':
                        Next();
                        Next();
                        TokenizeBinaryNumber();
                        break;
                    case 'o' or 'O':
                        Next();
                        Next();
                        TokenizeOctalNumber();
                        break;
                    case 'x' or 'X':
                        Next();
                        Next();
                        TokenizeHexadecimalNumber();
                        break;
                    default:
                        TokenizeDecimalNumber();
                        break;
                }

                state = LexerState.Operator;
            }
            else if (currentChar == '\'')
            {
                Next();
                TokenizeSingleString();
                state = LexerState.Operator;
            }
            else if (currentChar == '"')
            {
                Next();
                TokenizeDoubleString();
                state = LexerState.Operator;
            }
            else if (currentChar == '`')
            {
                Next();
                TokenizeVerbatimString();
                state = LexerState.Operator;
            }
            else if (IsIdentifierStart(currentChar))
            {
                TokenizeIdentifier();
                state = LexerState.Operator;
            }
            else if (state == LexerState.Value && currentChar == '/')
            {
                Next(); // skip first slash
                TokenizeRegex();
                state = LexerState.Operator;
            }
            else if (state == LexerState.Operator && currentChar == '!' && nextChar == 'i' && Peek(2) == 'n')
            {
                Next();
                Next();
                Next();
                AddToken(TokenType.NotIn, "!in");
                state = LexerState.Value;
            }
            else if (state == LexerState.Operator && currentChar == '!' && nextChar == 'i' && Peek(2) == 's')
            {
                Next();
                Next();
                Next();
                AddToken(TokenType.IsNot, "!is");
                state = LexerState.Value;
            }
            else if (state == LexerState.Operator && currentChar == '?' && nextChar == 'a' && Peek(2) == 's')
            {
                Next();
                Next();
                Next();
                AddToken(TokenType.NullableAs, "?as");
                state = LexerState.Value;
            }
            else if (state == LexerState.Operator && _operatorChars.Contains(currentChar))
            {
                if (currentChar == '/' && nextChar == '*')
                {
                    Next();
                    Next();
                    TokenizeMultilineComment();
                }
                else
                {
                    TokenizeOperator();
                    state = LexerState.Value;
                }
            }
            else if (state == LexerState.Value && _valueOperatorChars.Contains(currentChar))
            {
                TokenizeOperator();
            }
            else if (_bracketChars.Contains(currentChar))
            {
                TokenizeOperator();
                state = LexerState.Operator;
            }
            else if (currentChar == '#')
            {
                Next();
                TokenizeComment();
            }
            else if (parenthesisParity == 0 && bracketParity == 0 && currentChar == '\n')
            {
                Next();
                if (_tokens.Count > 0)
                {
                    var lastToken = _tokens.Last();
                    if (lastToken.Type != TokenType.LeftParen
                        && lastToken.Type != TokenType.LeftBracket
                        && lastToken.Type != TokenType.LeftCurly)
                    {
                        AddToken(TokenType.EndOfLine, "EOL");
                    }
                }
            }
            else
            {
                // Whitespace
                Next();
                continue;
            }

            if (_tokens.Count > 0)
            {
                var lastToken = _tokens.Last();
                if (lastToken.Type == TokenType.LeftParen)
                {
                    parenthesisParity++;
                }
                else if (lastToken.Type == TokenType.RightParen)
                {
                    parenthesisParity--;
                }
                else if (lastToken.Type == TokenType.LeftBracket)
                {
                    bracketParity++;
                }
                else if (lastToken.Type == TokenType.LeftBracket)
                {
                    bracketParity--;
                }
            }
        }

        if (_tokens.Count > 0)
        {
            var lastToken = _tokens.Last();
            if (lastToken.Type != TokenType.EndOfLine)
            {
                AddToken(TokenType.EndOfLine);
            }
        }

        AddToken(TokenType.EndOfFile);

        return _tokens;
    }

    private void TokenizeDecimalNumber()
    {
        _buffer.Clear();

        var current = Current();
        while (HasChars())
        {
            if (current is 'f' or 'F')
            {
                _buffer.Append('f');
                Next();
                break;
            }

            if (current is 'c' or 'C')
            {
                _buffer.Append('c');
                Next();
                break;
            }

            if (current is '_')
            {
                Next();
                continue;
            }

            if (current == '.')
            {
                if (_buffer.Length == 0)
                {
                    _buffer.Append('0');
                }
                else if (_buffer.ToString().Contains('.') && IsDigit(Peek(1)))
                {
                    throw new Exception("Syntax error: encountered second decimal point in a number literal");
                }
                else if (!IsDigit(Peek(1)))
                {
                    break;
                }
            }

            if (!IsDigit(current) && current is not '_' and not '.')
            {
                break;
            }

            _buffer.Append(current);
            current = Next();
        }

        AddToken(TokenType.NumberLiteral, _buffer.ToString());
    }

    private void TokenizeBinaryNumber()
    {
        _buffer.Clear();

        var current = Current();
        while (current is '0' or '1' or '_')
        {
            if (current == '_')
            {
                current = Next();
                continue;
            }

            _buffer.Append(current);
            current = Next();
        }

        AddToken(TokenType.NumberLiteral, Convert.ToInt64(_buffer.ToString(), 2).ToString());
    }

    private void TokenizeOctalNumber()
    {
        _buffer.Clear();

        var current = Current();
        while (current is >= '0' and <= '7' or '_')
        {
            if (current == '_')
            {
                current = Next();
                continue;
            }

            _buffer.Append(current);
            current = Next();
        }

        AddToken(TokenType.NumberLiteral, Convert.ToInt64(_buffer.ToString(), 8).ToString());
    }

    private void TokenizeHexadecimalNumber()
    {
        _buffer.Clear();

        var current = Current();
        while (current is >= '0' and <= '9' or >= 'a' and <= 'f' or >= 'A' and <= 'F' or '_')
        {
            if (current == '_')
            {
                current = Next();
                continue;
            }

            _buffer.Append(current);
            current = Next();
        }

        AddToken(TokenType.NumberLiteral, Convert.ToInt64(_buffer.ToString(), 16).ToString());
    }

    private void TokenizeSingleString()
    {
        _buffer.Clear();
        var currentChar = Current();
        var unclosedCurlies = 0;
        while (HasChars())
        {
            switch (currentChar)
            {
                case '{':
                    currentChar = Next();
                    unclosedCurlies++;
                    _buffer.Append('{');
                    continue;
                case '}':
                    currentChar = Next();
                    unclosedCurlies--;
                    _buffer.Append('}');
                    continue;
                case '\\':
                    currentChar = Next();
                    switch (currentChar)
                    {
                        case '$':
                            _buffer.Append("\\$");
                            break;
                        case '\'':
                            _buffer.Append('\'');
                            break;
                        case '\\':
                            _buffer.Append('\\');
                            break;
                        case 'n':
                            _buffer.Append('\n');
                            break;
                        case 'r':
                            _buffer.Append('\r');
                            break;
                        case 't':
                            _buffer.Append('\t');
                            break;
                        case 'b':
                            _buffer.Append('\b');
                            break;
                        case '0':
                            _buffer.Append('\0');
                            break;
                        default:
                            throw new Exception($"Unrecognized escape sequence: \\{currentChar}");
                    }

                    currentChar = Next();
                    break;
            }

            if (currentChar == '\'' && unclosedCurlies == 0)
                break;
            if (currentChar == '\0')
                throw new Exception("Unclosed string");
            _buffer.Append(currentChar);
            currentChar = Next();
        }

        if (unclosedCurlies != 0)
            throw new Exception("Syntax error: Unbalanced curly brackets inside template string");
        Next(); // Skip closing quote
        AddToken(TokenType.StringLiteral, _buffer.ToString());
    }

    private void TokenizeDoubleString()
    {
        _buffer.Clear();
        var currentChar = Current();
        var unclosedCurlies = 0;
        while (HasChars())
        {
            switch (currentChar)
            {
                case '{':
                    currentChar = Next();
                    unclosedCurlies++;
                    _buffer.Append('{');
                    continue;
                case '}':
                    currentChar = Next();
                    unclosedCurlies--;
                    _buffer.Append('}');
                    continue;
                case '\\':
                    currentChar = Next();
                    switch (currentChar)
                    {
                        case '$':
                            _buffer.Append("\\$");
                            break;
                        case '\'':
                            _buffer.Append('\'');
                            break;
                        case '\\':
                            _buffer.Append('\\');
                            break;
                        case 'n':
                            _buffer.Append('\n');
                            break;
                        case 'r':
                            _buffer.Append('\r');
                            break;
                        case 't':
                            _buffer.Append('\t');
                            break;
                        case 'b':
                            _buffer.Append('\b');
                            break;
                        case '0':
                            _buffer.Append('\0');
                            break;
                        default:
                            throw new Exception($"Unrecognized escape sequence: \\{currentChar}");
                    }

                    currentChar = Next();
                    break;
            }

            if (currentChar == '"' && unclosedCurlies == 0)
                break;
            if (currentChar == '\0')
                throw new Exception("Unclosed string");
            _buffer.Append(currentChar);
            currentChar = Next();
        }

        if (unclosedCurlies != 0)
            throw new Exception("Syntax error: Unbalanced curly brackets inside template string");
        Next(); // Skip closing quote
        AddToken(TokenType.StringLiteral, _buffer.ToString());
    }

    private void TokenizeVerbatimString()
    {
        _buffer.Clear();
        var currentChar = Current();
        while (HasChars())
        {
            if (currentChar == '`')
                break;
            if (currentChar == '\0')
                throw new Exception("Unclosed string");
            _buffer.Append(currentChar);
            currentChar = Next();
        }

        Next(); // Skip closing quote
        AddToken(TokenType.StringLiteral, _buffer.ToString());
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
        AddToken(_operatorMap[op]);
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

        var ident = _buffer.ToString();
        if (_keywordMap.TryGetValue(ident, out var token))
        {
            AddToken(token);
        }
        else
        {
            AddToken(TokenType.Identifier, ident);
        }
    }

    private void TokenizeComment()
    {
        var ch = Current();
        while (HasChars())
        {
            if (ch == '\n') break;
            ch = Next();
        }
    }

    private void TokenizeMultilineComment()
    {
        var ch = Current();
        while (HasChars())
        {
            if (ch == '*' && Peek(1) == '/') break;
            ch = Next();
        }
    }

    private void TokenizeRegex()
    {
        _buffer.Clear();
        var ch = Current();
        var flags = "";
        // parse pattern part (anything until unescaped / is found)
        while (HasChars())
        {
            if (ch == '/') break;
            _buffer.Append(ch);
            ch = Next();
        }

        ch = Next(); // skip final /
        // parse flags part (any string of subsequent letters)
        while (HasChars())
        {
            if (char.ToLower(ch) is < 'a' or > 'z') break;
            flags += ch;
            ch = Next();
        }

        AddToken(TokenType.RegexLiteral, $"/{_buffer}/{flags}");
    }

    private void AddToken(TokenType type, string? value = null)
    {
        _tokens.Add(new Token(_cursor.Clone(), type, value));
    }

    private bool IsIdentifierStart(char ch)
    {
        return ch is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_' or '$';
    }

    private bool IsIdentifierContinuation(char ch)
    {
        return ch is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '_' or '$';
    }

    private static bool IsDigit(char ch)
    {
        return ch is >= '0' and <= '9';
    }

    private char Next()
    {
        _cursor.Position++;

        if (Current() == '\n')
        {
            _cursor.Line++;
            _cursor.Column = 1;
            // _cursor.Position++; // skip newline
        }

        return Current();
    }

    private char Current()
    {
        return Peek(0);
    }

    private char Peek(int relative)
    {
        var pos = _cursor.Position + relative;
        return pos < source.Length ? source[pos] : '\0';
    }
}