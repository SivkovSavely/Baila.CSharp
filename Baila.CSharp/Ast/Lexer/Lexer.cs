using System.Text;
using Baila.CSharp.Ast.Diagnostics;
using Baila.CSharp.Ast.Syntax;

namespace Baila.CSharp.Lexer;

public class Lexer(
    string source,
    string filename,
    LexerMode mode = LexerMode.Regular,
    CancellationToken? cancellationToken = null)
{
    private int _startColumn = 1;
    private int _position = 0;
    private int _column = 1;
    private int _line = 1;
    private string _filename = filename;
    private readonly List<Token> _tokens = [];
    private readonly StringBuilder _buffer = new(256);
    private readonly List<LexerDiagnostic> _diagnostics = [];
    private readonly string _source = source.Replace("\r\n", "\n");
    private readonly string[] _lines = source.Replace("\r\n", "\n").Split("\n").ToArray();

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

    private bool HasChars() => _position < _source.Length;

    public IEnumerable<LexerDiagnostic> Diagnostics => _diagnostics.AsReadOnly();

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
            _startColumn = _column;
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
                TokenizeSingleOrDoubleString('\'');
                state = LexerState.Operator;
            }
            else if (currentChar == '"')
            {
                Next();
                TokenizeSingleOrDoubleString('"');
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
            else if (currentChar == '\n')
            {
                if (parenthesisParity == 0 && bracketParity == 0)
                {
                    Next();
                    if (_tokens.Count > 0 && mode != LexerMode.InterpolatedString)
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
                else if (mode is LexerMode.Highlighting or LexerMode.HighlightingInterpolatedString)
                {
                    AddToken(TokenType.Whitespace, currentChar.ToString());
                    Next();
                }
                else
                {
                    Next();
                }
            }
            else
            {
                // Whitespace
                var ch = Current();
                
                if (mode is LexerMode.Highlighting or LexerMode.HighlightingInterpolatedString)
                {
                    AddToken(TokenType.Whitespace, ch.ToString());
                }
                
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
        
        if (mode == LexerMode.Regular)
        {
            if (_tokens.Count > 0)
            {
                var lastToken = _tokens.Last();
                if (lastToken.Type != TokenType.EndOfLine)
                {
                    AddToken(TokenType.EndOfLine);
                }
            }

            AddToken(TokenType.EndOfFile);
        }

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
                    AddDiagnostic(LexerDiagnostics.BL0001_SecondDecimalPoint);
                    break;
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

        AddToken(TokenType.NumberLiteral, _buffer.ToString(), tokenLengthOffset: -1);
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

        AddToken(TokenType.NumberLiteral, Convert.ToInt64(_buffer.ToString(), 2).ToString(), tokenLengthOffset: -1);
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

        AddToken(TokenType.NumberLiteral, Convert.ToInt64(_buffer.ToString(), 8).ToString(), tokenLengthOffset: -1);
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

        AddToken(TokenType.NumberLiteral, Convert.ToInt64(_buffer.ToString(), 16).ToString(), tokenLengthOffset: -1);
    }

    private void TokenizeSingleOrDoubleString(char quoteChar)
    {
        _buffer.Clear();
        var stringStartColumn = _startColumn;
        var stringEndColumn = _startColumn;
        var fixedStringLiteralStartColumn = _startColumn;
        var expressionStartColumn = _startColumn;
        var currentChar = Current();
        var unclosedCurlies = 0;
        var isInterpolated = false;
        var isTokenizingSimpleIdentifierInterpolation = false;
        List<Token>? interpolatedStringTokens = null;
        var previousColumn = _column;
        var previousLine = _line;
        while (true)
        {
            if (isTokenizingSimpleIdentifierInterpolation)
            {
                if (!IsIdentifierContinuation(currentChar))
                {
                    isTokenizingSimpleIdentifierInterpolation = false;
                    if (interpolatedStringTokens!.Count != 0)
                    {
                        interpolatedStringTokens.Add(CreateToken(TokenType.Comma));
                    }
                    interpolatedStringTokens.Add(
                        CreateToken(TokenType.Identifier, _buffer.ToString()));
                    _buffer.Clear();
                    continue;
                }

                _buffer.Append(currentChar);
                previousColumn = _column;
                previousLine = _line;
                currentChar = Next();
                continue;
            }
            
            switch (currentChar)
            {
                case '$' when Peek(1) == '{':
                    // Add the fixed string literal that was before ${, if there was none, still add an empty one
                    isInterpolated = true;
                    Next(); // skip $
                    currentChar = Next(); // skip {
                    unclosedCurlies++;
                    interpolatedStringTokens ??= [];
                    _startColumn = fixedStringLiteralStartColumn;
                    expressionStartColumn = _column;
                    if (_buffer.Length > 0)
                    {
                        if (interpolatedStringTokens.Count != 0)
                        {
                            interpolatedStringTokens.Add(CreateToken(TokenType.Comma));
                        }
                        interpolatedStringTokens.Add(
                            CreateToken(
                                mode is LexerMode.Highlighting or LexerMode.HighlightingInterpolatedString
                                    ? quoteChar == '\''
                                        ? TokenType.SingleQuoteStringLiteral
                                        : TokenType.DoubleQuoteStringLiteral
                                    : TokenType.StringLiteral,
                                _buffer.ToString()));
                        _buffer.Clear();
                    }

                    continue;
                case '$' when IsIdentifierStart(Peek(1)):
                    isInterpolated = true;
                    isTokenizingSimpleIdentifierInterpolation = true;
                    currentChar = Next(); // skip $
                    interpolatedStringTokens ??= [];
                    if (_buffer.Length > 0)
                    {
                        if (interpolatedStringTokens.Count != 0)
                        {
                            interpolatedStringTokens.Add(CreateToken(TokenType.Comma));
                        }
                        interpolatedStringTokens.Add(
                            CreateToken(TokenType.StringLiteral, _buffer.ToString()));
                        _buffer.Clear();
                    }

                    continue;
                case '}':
                    var expressionEndColumn = _column - 1; // not counting the }
                    currentChar = Next(); // skip }
                    unclosedCurlies--;
                    if (unclosedCurlies == 0)
                    {
                        if (_buffer.Length == 0)
                        {
                            AddDiagnostic(LexerDiagnostics.BL0002_EmptyInterpolatedStringExpression, columnOffset: -1);
                            break;
                        }

                        if (interpolatedStringTokens == null)
                        {
                            AddDiagnostic(LexerDiagnostics.BL0003_UnexpectedStringInterpolationExpressionClosingBrace);
                            break;
                        }

                        if (interpolatedStringTokens.Count != 0)
                        {
                            interpolatedStringTokens.Add(CreateToken(TokenType.Comma));
                        }

                        var lexer = new Lexer(
                            _buffer.ToString(),
                            _filename,
                            mode is LexerMode.Highlighting or LexerMode.HighlightingInterpolatedString
                                ? LexerMode.HighlightingInterpolatedString
                                : LexerMode.InterpolatedString);
                        var expressionTokens = lexer.Tokenize();
                        interpolatedStringTokens.Add(
                            CreateToken(
                                    TokenType.LeftParen,
                                    syntaxNodeSpanOverride: SyntaxNodeSpan.FromEnd(
                                        _filename, _line, expressionStartColumn, _line, expressionStartColumn)));
                        interpolatedStringTokens.AddRange(
                            expressionTokens.Select(
                                expressionToken => CreateToken(
                                    expressionToken.Type,
                                    expressionToken.Value,
                                    syntaxNodeSpanOverride: SyntaxNodeSpan.FromEnd(
                                        _filename,
                                        expressionToken.Span.StartLine,
                                        expressionToken.Span.StartColumn + expressionStartColumn - 1,
                                        expressionToken.Span.EndLine,
                                        expressionToken.Span.EndColumn + expressionStartColumn - 1))));
                        interpolatedStringTokens.Add(
                            CreateToken(
                                    TokenType.RightParen,
                                    syntaxNodeSpanOverride: SyntaxNodeSpan.FromEnd(
                                        _filename, _line, expressionEndColumn, _line, expressionEndColumn)));
                        _buffer.Clear();
                    }

                    continue;
                case '\\':
                    currentChar = Next();
                    switch (currentChar)
                    {
                        case '$':
                            _buffer.Append(mode is LexerMode.Highlighting or LexerMode.HighlightingInterpolatedString ? @"\\$" : "\\$");
                            break;
                        case '\'' when quoteChar == '\'':
                            _buffer.Append(mode is LexerMode.Highlighting or LexerMode.HighlightingInterpolatedString ? @"\'" : '\'');
                            break;
                        case '\"' when quoteChar == '\"':
                            _buffer.Append(mode is LexerMode.Highlighting or LexerMode.HighlightingInterpolatedString ? @"""" : '"');
                            break;
                        case '\\':
                            _buffer.Append(mode is LexerMode.Highlighting or LexerMode.HighlightingInterpolatedString ? @"\\" : '\\');
                            break;
                        case 'n':
                            _buffer.Append(mode is LexerMode.Highlighting or LexerMode.HighlightingInterpolatedString ? @"\n" : '\n');
                            break;
                        case 'r':
                            _buffer.Append(mode is LexerMode.Highlighting or LexerMode.HighlightingInterpolatedString ? @"\r" : '\r');
                            break;
                        case 't':
                            _buffer.Append(mode is LexerMode.Highlighting or LexerMode.HighlightingInterpolatedString ? @"\t" : '\t');
                            break;
                        case 'b':
                            _buffer.Append(mode is LexerMode.Highlighting or LexerMode.HighlightingInterpolatedString ? @"\b" : '\b');
                            break;
                        case '0':
                            _buffer.Append(mode is LexerMode.Highlighting or LexerMode.HighlightingInterpolatedString ? @"\0" : '\0');
                            break;
                        default:
                            AddDiagnostic(LexerDiagnostics.BL0004_UnrecognizedStringEscapeSequence, currentChar);
                            break;
                    }

                    previousColumn = _column;
                    previousLine = _line;
                    currentChar = Next();
                    continue;
            }

            if (currentChar == quoteChar && unclosedCurlies == 0)
            {
                stringEndColumn = _column;
                break;
            }
            if (currentChar is '\n' or '\0')
            {
                AddDiagnosticWithSpan(LexerDiagnostics.BL0006_UnclosedString, previousLine, stringStartColumn, previousColumn);
                break;
            }
            _buffer.Append(currentChar);
            previousColumn = _column;
            previousLine = _line;
            currentChar = Next();
        }

        var line = _line;

        if (unclosedCurlies != 0)
        {
            AddDiagnostic(LexerDiagnostics.BL0005_UnbalancedBracesInsideTemplateString);
            return;
        }

        Next(); // Skip closing quote

        if (isInterpolated)
        {
            if (_buffer.Length > 0)
            {
                if (interpolatedStringTokens!.Count != 0)
                {
                    interpolatedStringTokens.Add(CreateToken(TokenType.Comma));
                }
                interpolatedStringTokens.Add(
                    CreateToken(
                        mode is LexerMode.Highlighting or LexerMode.HighlightingInterpolatedString
                            ? quoteChar == '"'
                                ? TokenType.DoubleQuoteStringLiteral
                                : TokenType.SingleQuoteStringLiteral
                            : TokenType.StringLiteral,
                        _buffer.ToString()));
                _buffer.Clear();
            }

            AddToken(TokenType.PrivateStringConcat);
            AddToken(
                TokenType.PrivateStringConcatStart,
                syntaxNodeSpanOverride: SyntaxNodeSpan.FromEnd(
                    _filename, line, stringStartColumn, line, stringStartColumn));
            _tokens.AddRange(interpolatedStringTokens!);
            AddToken(
                TokenType.PrivateStringConcatEnd,
                syntaxNodeSpanOverride: SyntaxNodeSpan.FromEnd(
                    _filename, line, stringEndColumn, line, stringEndColumn));
        }
        else if (mode is LexerMode.Highlighting or LexerMode.HighlightingInterpolatedString)
        {
            AddToken(
                quoteChar == '"'
                    ? TokenType.DoubleQuoteStringLiteral
                    : TokenType.SingleQuoteStringLiteral,
                _buffer.ToString());
        }
        else
        {
            AddToken(TokenType.StringLiteral, _buffer.ToString(), tokenLengthOffset: -1);
        }
    }

    private void TokenizeVerbatimString()
    {
        _buffer.Clear();
        var startColumn = _column;
        var currentChar = Current();
        while (HasChars())
        {
            switch (currentChar)
            {
                case '\\' when Peek(1) == '`':
                    Next(); // skip \
                    currentChar = Next(); // skip `
                    _buffer.Append(@"\`");
                    continue;
            }
            
            if (currentChar == '`')
                break;
            if (currentChar == '\0')
            {
                AddDiagnosticWithSpan(LexerDiagnostics.BL0006_UnclosedString, _line, startColumn, _column);
                break;
            }
            _buffer.Append(currentChar);
            currentChar = Next();
        }

        Next(); // Skip closing quote

        if (mode is LexerMode.Highlighting or LexerMode.HighlightingInterpolatedString)
        {
            AddToken(TokenType.BacktickStringLiteral, _buffer.ToString());
        }
        else
        {
            AddToken(TokenType.StringLiteral, _buffer.ToString());
        }
    }

    private void TokenizeOperator()
    {
        _buffer.Clear();
        var startLine = _line;
        var startColumn = _startColumn;

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
        AddToken(
            _operatorMap[op],
            syntaxNodeSpanOverride: new SyntaxNodeSpan(_filename, startLine, startColumn, 1, op.Length));
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

        StringBuilder? buffer = null;
        if (mode is LexerMode.Highlighting or LexerMode.HighlightingInterpolatedString) buffer = new StringBuilder();
        
        while (HasChars())
        {
            if (ch == '\n') break;
            buffer?.Append(ch);
            ch = Next();
        }

        if (mode is LexerMode.Highlighting or LexerMode.HighlightingInterpolatedString)
        {
            AddToken(TokenType.SingleLineComment, buffer!.ToString());
        }
    }

    private void TokenizeMultilineComment()
    {
        var ch = Current();

        StringBuilder? buffer = null;
        if (mode is LexerMode.Highlighting or LexerMode.HighlightingInterpolatedString) buffer = new StringBuilder();

        while (HasChars())
        {
            if (ch == '*' && Peek(1) == '/')
            {
                Next(); // skip *
                Next(); // skip /
                break;
            }
            buffer?.Append(ch);
            ch = Next();
        }

        if (mode is LexerMode.Highlighting or LexerMode.HighlightingInterpolatedString)
        {
            AddToken(TokenType.MultiLineComment, buffer!.ToString());
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

    private Token CreateToken(TokenType type, string? value = null, int tokenLengthOffset = 0, SyntaxNodeSpan? syntaxNodeSpanOverride = null)
    {
        var tokenSpan = syntaxNodeSpanOverride ?? new SyntaxNodeSpan(_filename, _line, _startColumn, 1, _column - _startColumn + 1 + tokenLengthOffset);
        return new Token(_filename, tokenSpan, type, value);
    }

    private void AddToken(TokenType type, string? value = null, int tokenLengthOffset = 0, SyntaxNodeSpan? syntaxNodeSpanOverride = null)
    {
        _tokens.Add(CreateToken(type, value, tokenLengthOffset, syntaxNodeSpanOverride));
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
        cancellationToken?.ThrowIfCancellationRequested();

        _position++;
        _column++;

        if (Current() == '\n')
        {
            _line++;
            _column = 1;
            // _position++; // skip newline
        }

        return Current();
    }

    private char Current()
    {
        cancellationToken?.ThrowIfCancellationRequested();

        return Peek(0);
    }

    private char Peek(int relative)
    {
        var pos = _position + relative;
        return pos < _source.Length ? _source[pos] : '\0';
    }

    private Cursor CreateCursor(int columnOffset = 0) => new(_position, _column + columnOffset, _line, _filename);

    private SyntaxNodeSpan CreateSpan(int line, int startColumn, int endColumn) => SyntaxNodeSpan.FromEnd(
        _filename, line, startColumn, line, endColumn);

    private string GetLine(int? lineNo = null)
    {
        lineNo ??= _line - 1;
        return _lines[lineNo.Value];
    }

    private void AddDiagnostic(Func<Cursor, string, LexerDiagnostic> diagnosticCreator, int columnOffset = 0)
    {
        var diagnostic = diagnosticCreator(CreateCursor(columnOffset), GetLine());
        _diagnostics.Add(diagnostic);
    }

    private void AddDiagnosticWithSpan(Func<SyntaxNodeSpan, string, LexerDiagnostic> diagnosticCreator, int line, int startColumn, int endColumn)
    {
        var diagnostic = diagnosticCreator(CreateSpan(line, startColumn, endColumn), GetLine(line - 1));
        _diagnostics.Add(diagnostic);
    }

    private void AddDiagnostic<TParam>(
        Func<TParam, Cursor, string, LexerDiagnostic> diagnosticCreator,
        TParam param,
        int columnOffset = 0)
    {
        var diagnostic = diagnosticCreator(param, CreateCursor(columnOffset), GetLine());
        _diagnostics.Add(diagnostic);
    }

    private void AddDiagnostic<TParam1, TParam2>(
        Func<TParam1, TParam2, Cursor, string, LexerDiagnostic> diagnosticCreator,
        TParam1 param1, TParam2 param2,
        int columnOffset = 0)
    {
        var diagnostic = diagnosticCreator(param1, param2, CreateCursor(columnOffset), GetLine());
        _diagnostics.Add(diagnostic);
    }

    private void AddDiagnostic<TParam1, TParam2, TParam3>(
        Func<TParam1, TParam2, TParam3, Cursor, string, LexerDiagnostic> diagnosticCreator,
        TParam1 param1, TParam2 param2, TParam3 param3,
        int columnOffset = 0)
    {
        var diagnostic = diagnosticCreator(param1, param2, param3, CreateCursor(columnOffset), GetLine());
        _diagnostics.Add(diagnostic);
    }

    private string TraceTokens()
    {
        var sb = new StringBuilder();

        var i = 0;
        foreach (var token in _source + "\0")
        {
            var prettyToken = token switch
            {
                ' ' => "[ ]",
                '\r' => @"[\r]",
                '\n' => @"[\n]",
                '\t' => @"[\t]",
                _ => token.ToString()
            };
            if (i++ == _position)
            {
                sb.Append(new string('>', 10));
                sb.Append(' ');
                sb.Append(prettyToken);
                sb.Append(' ');
                sb.AppendLine(new string('<', 10));
            }
            else
            {
                sb.Append(new string(' ', 11));
                sb.AppendLine(prettyToken);
            }
        }
        
        return sb.ToString();
    }
}