namespace Baila.CSharp.Lexer;

public record TokenType(
    string Type,
    bool HasMeaningfulValue = false,
    bool IsOperator = false,
    bool IsKeyword = false)
{
    public static readonly TokenType EndOfFile = new("EOF");
    public static readonly TokenType EndOfLine = new("EOL");

    /// <summary>
    /// Only used in LexerMode.Highlighting
    /// </summary>
    public static readonly TokenType Whitespace = new("WHITESPACE", HasMeaningfulValue: true);
    /// <summary>
    /// Only used in LexerMode.Highlighting
    /// </summary>
    public static readonly TokenType SingleLineComment = new("SINGLE_LINE_COMMENT", HasMeaningfulValue: true);
    /// <summary>
    /// Only used in LexerMode.Highlighting
    /// </summary>
    public static readonly TokenType MultiLineComment = new("MULTI_LINE_COMMENT", HasMeaningfulValue: true);

    public static readonly TokenType Identifier = new("IDENTIFIER", HasMeaningfulValue: true);
    public static readonly TokenType StringLiteral = new("STRING", HasMeaningfulValue: true);
    public static readonly TokenType SingleQuoteStringLiteral = new("'STRING'", HasMeaningfulValue: true);
    public static readonly TokenType DoubleQuoteStringLiteral = new("\"STRING\"", HasMeaningfulValue: true);
    public static readonly TokenType BacktickStringLiteral = new("`STRING`", HasMeaningfulValue: true);
    public static readonly TokenType StringLiteralEscapeSequence = new("ESCAPE_SEQUENCE", HasMeaningfulValue: true);
    public static readonly TokenType NumberLiteral = new("NUMBER", HasMeaningfulValue: true);
    public static readonly TokenType RegexLiteral = new("REGEX", HasMeaningfulValue: true);

    // operators
    public static readonly TokenType Plus = new("+", IsOperator: true);
    public static readonly TokenType PlusPlus = new("++", IsOperator: true);
    public static readonly TokenType Minus = new("-", IsOperator: true);
    public static readonly TokenType MinusMinus = new("--", IsOperator: true);
    public static readonly TokenType Star = new("*", IsOperator: true);
    public static readonly TokenType StarStar = new("**", IsOperator: true);
    public static readonly TokenType Slash = new("/", IsOperator: true);
    public static readonly TokenType SlashSlash = new("//", IsOperator: true);
    public static readonly TokenType Percent = new("%", IsOperator: true);

    public static readonly TokenType Eq = new("=", IsOperator: true);
    public static readonly TokenType EqEq = new("==", IsOperator: true);
    public static readonly TokenType EqEqEq = new("===", IsOperator: true);
    public static readonly TokenType ExclEq = new("!=", IsOperator: true);
    public static readonly TokenType ExclEqEq = new("!==", IsOperator: true);
    public static readonly TokenType PlusEq = new("+=", IsOperator: true);
    public static readonly TokenType MinusEq = new("-=", IsOperator: true);
    public static readonly TokenType StarEq = new("*=", IsOperator: true);
    public static readonly TokenType StarStarEq = new("**=", IsOperator: true);
    public static readonly TokenType SlashEq = new("/=", IsOperator: true);
    public static readonly TokenType SlashSlashEq = new("//=", IsOperator: true);
    public static readonly TokenType PercentEq = new("%=", IsOperator: true);
    public static readonly TokenType BarEq = new("|=", IsOperator: true);
    public static readonly TokenType BarBarEq = new("||=", IsOperator: true);
    public static readonly TokenType AmpEq = new("&=", IsOperator: true);
    public static readonly TokenType AmpAmpEq = new("&&=", IsOperator: true);
    public static readonly TokenType CaretEq = new("^=", IsOperator: true);
    public static readonly TokenType CaretCaretEq = new("^=", IsOperator: true);

    public static readonly TokenType Bar = new("|", IsOperator: true);
    public static readonly TokenType BarBar = new("||", IsOperator: true);
    public static readonly TokenType Amp = new("&", IsOperator: true);
    public static readonly TokenType AmpAmp = new("&&", IsOperator: true);
    public static readonly TokenType Pipe = new("|>", IsOperator: true);
    public static readonly TokenType Caret = new("^", IsOperator: true);
    public static readonly TokenType CaretCaret = new("^^", IsOperator: true);
    public static readonly TokenType Tilde = new("~", IsOperator: true);

    public static readonly TokenType Dot = new(".", IsOperator: true);
    public static readonly TokenType DotDot = new("", IsOperator: true);
    public static readonly TokenType NullDot = new("?.", IsOperator: true);
    public static readonly TokenType Comma = new(",", IsOperator: true);
    public static readonly TokenType Elvis = new("??", IsOperator: true);
    public static readonly TokenType ElvisEq = new("??=", IsOperator: true);
    public static readonly TokenType Excl = new("!", IsOperator: true);

    public static readonly TokenType Lt = new("<", IsOperator: true);
    public static readonly TokenType LtEq = new("<=", IsOperator: true);
    public static readonly TokenType LtLt = new("<<", IsOperator: true);
    public static readonly TokenType LtLtEq = new("<<=", IsOperator: true);
    public static readonly TokenType Gt = new(">", IsOperator: true);
    public static readonly TokenType GtEq = new(">=", IsOperator: true);
    public static readonly TokenType GtGt = new(">>", IsOperator: true);
    public static readonly TokenType GtGtEq = new(">>=", IsOperator: true);
    public static readonly TokenType GtGtGt = new(">>>", IsOperator: true);
    public static readonly TokenType GtGtGtEq = new(">>>=", IsOperator: true);

    public static readonly TokenType DoubleArrow = new("=>", IsOperator: true);
    public static readonly TokenType Semicolon = new(";", IsOperator: true);

    public static readonly TokenType SingleArrow = new("->", IsOperator: true);
    public static readonly TokenType Question = new("?", IsOperator: true);
    public static readonly TokenType Colon = new(":", IsOperator: true);
    public static readonly TokenType ColonColon = new("::", IsOperator: true);

    public static readonly TokenType LeftParen = new("(", IsOperator: true);
    public static readonly TokenType RightParen = new(")", IsOperator: true);
    public static readonly TokenType LeftBracket = new("[", IsOperator: true);
    public static readonly TokenType RightBracket = new("]", IsOperator: true);
    public static readonly TokenType LeftCurly = new("{", IsOperator: true);
    public static readonly TokenType RightCurly = new("}", IsOperator: true);

    // predefined object literal keywords
    public static readonly TokenType True = new("true", IsKeyword: true);
    public static readonly TokenType False = new("false", IsKeyword: true);
    public static readonly TokenType Null = new("null", IsKeyword: true);
    public static readonly TokenType This = new("this", IsKeyword: true);
    public static readonly TokenType Super = new("super", IsKeyword: true);

    public static readonly TokenType Var = new("var", IsKeyword: true);
    public static readonly TokenType Const = new("const", IsKeyword: true);
    public static readonly TokenType Property = new("property", IsKeyword: true);
    public static readonly TokenType Function = new("function", IsKeyword: true);
    public static readonly TokenType Typeof = new("typeof", IsKeyword: true);
    public static readonly TokenType From = new("from", IsKeyword: true);
    public static readonly TokenType Import = new("import", IsKeyword: true);
    public static readonly TokenType Export = new("export", IsKeyword: true);
    public static readonly TokenType Ref = new("ref", IsKeyword: true);
    public static readonly TokenType Operator = new("operator", IsKeyword: true);

    public static readonly TokenType Constructor = new("constructor", IsKeyword: true);
    public static readonly TokenType Deconstructor = new("deconstructor", IsKeyword: true);

    public static readonly TokenType Public = new("public", IsKeyword: true);
    public static readonly TokenType Private = new("private", IsKeyword: true);
    public static readonly TokenType Protected = new("protected", IsKeyword: true);
    public static readonly TokenType Sealed = new("sealed", IsKeyword: true);
    public static readonly TokenType Static = new("static", IsKeyword: true);
    public static readonly TokenType Global = new("global", IsKeyword: true);

    public static readonly TokenType Async = new("async", IsKeyword: true);
    public static readonly TokenType Await = new("await", IsKeyword: true);
    public static readonly TokenType Class = new("class", IsKeyword: true);
    public static readonly TokenType Struct = new("struct", IsKeyword: true);
    public static readonly TokenType Interface = new("interface", IsKeyword: true);
    public static readonly TokenType Enum = new("enum", IsKeyword: true);

    public static readonly TokenType Override = new("override", IsKeyword: true);

    // control flow keywords
    public static readonly TokenType If = new("if", IsKeyword: true);
    public static readonly TokenType Else = new("else", IsKeyword: true);
    public static readonly TokenType Switch = new("switch", IsKeyword: true);
    public static readonly TokenType Match = new("match", IsKeyword: true);
    public static readonly TokenType For = new("for", IsKeyword: true);
    public static readonly TokenType Foreach = new("foreach", IsKeyword: true);
    public static readonly TokenType Do = new("do", IsKeyword: true);
    public static readonly TokenType While = new("while", IsKeyword: true);
    public static readonly TokenType Try = new("try", IsKeyword: true);
    public static readonly TokenType Catch = new("catch", IsKeyword: true);
    public static readonly TokenType Finally = new("finally", IsKeyword: true);
    public static readonly TokenType Break = new("break", IsKeyword: true);
    public static readonly TokenType Continue = new("continue", IsKeyword: true);
    public static readonly TokenType Return = new("return", IsKeyword: true);
    public static readonly TokenType Throw = new("throw", IsKeyword: true);
    public static readonly TokenType Yield = new("yield", IsKeyword: true);

    // operator keywords
    public static readonly TokenType In = new("in", IsKeyword: true);
    public static readonly TokenType NotIn = new("!in", IsKeyword: true);
    public static readonly TokenType Is = new("is", IsKeyword: true);
    public static readonly TokenType IsNot = new("!is", IsKeyword: true);
    public static readonly TokenType As = new("as", IsKeyword: true);
    public static readonly TokenType NullableAs = new("?as", IsKeyword: true);
    
    // private tokens: these are reserved for internal language use and are not able to be lexed from source code
    public static readonly TokenType PrivateStringConcat = new("[[string_concat]]");
    public static readonly TokenType PrivateStringConcatStart = new("[[string_concat_start]]");
    public static readonly TokenType PrivateStringConcatEnd = new("[[string_concat_end]]");
}