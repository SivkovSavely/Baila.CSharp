namespace Baila.CSharp.Lexer;

public record TokenType(string Type)
{
    public static readonly TokenType EndOfFile = new("EOF");
    public static readonly TokenType EndOfLine = new("EOL");

    public static readonly TokenType Identifier = new("IDENTIFIER");
    public static readonly TokenType StringLiteral = new("STRING");
    public static readonly TokenType NumberLiteral = new("NUMBER");
    public static readonly TokenType RegexLiteral = new("REGEX");

    // operators
    public static readonly TokenType Plus = new("+");
    public static readonly TokenType PlusPlus = new("++");
    public static readonly TokenType Minus = new("-");
    public static readonly TokenType MinusMinus = new("--");
    public static readonly TokenType Star = new("*");
    public static readonly TokenType StarStar = new("**");
    public static readonly TokenType Slash = new("/");
    public static readonly TokenType SlashSlash = new("//");
    public static readonly TokenType Percent = new("%");

    public static readonly TokenType Eq = new("=");
    public static readonly TokenType EqEq = new("==");
    public static readonly TokenType EqEqEq = new("===");
    public static readonly TokenType ExclEq = new("!=");
    public static readonly TokenType ExclEqEq = new("!==");
    public static readonly TokenType PlusEq = new("+=");
    public static readonly TokenType MinusEq = new("-=");
    public static readonly TokenType StarEq = new("*=");
    public static readonly TokenType StarStarEq = new("**=");
    public static readonly TokenType SlashEq = new("/=");
    public static readonly TokenType SlashSlashEq = new("//=");
    public static readonly TokenType PercentEq = new("%=");
    public static readonly TokenType BarEq = new("|=");
    public static readonly TokenType BarBarEq = new("||=");
    public static readonly TokenType AmpEq = new("&=");
    public static readonly TokenType AmpAmpEq = new("&&=");
    public static readonly TokenType CaretEq = new("^=");
    public static readonly TokenType CaretCaretEq = new("^=");

    public static readonly TokenType Bar = new("|");
    public static readonly TokenType BarBar = new("||");
    public static readonly TokenType Amp = new("&");
    public static readonly TokenType AmpAmp = new("&&");
    public static readonly TokenType Pipe = new("|>");
    public static readonly TokenType Caret = new("^");
    public static readonly TokenType CaretCaret = new("^^");
    public static readonly TokenType Tilde = new("~");

    public static readonly TokenType Dot = new(".");
    public static readonly TokenType DotDot = new("");
    public static readonly TokenType NullDot = new("?.");
    public static readonly TokenType Comma = new(",");
    public static readonly TokenType Elvis = new("??");
    public static readonly TokenType ElvisEq = new("??=");
    public static readonly TokenType Excl = new("!");

    public static readonly TokenType Lt = new("<");
    public static readonly TokenType LtEq = new("<=");
    public static readonly TokenType LtLt = new("<<");
    public static readonly TokenType LtLtEq = new("<<=");
    public static readonly TokenType Gt = new(">");
    public static readonly TokenType GtEq = new(">=");
    public static readonly TokenType GtGt = new(">>");
    public static readonly TokenType GtGtEq = new(">>=");
    public static readonly TokenType GtGtGt = new(">>>");
    public static readonly TokenType GtGtGtEq = new(">>>=");

    public static readonly TokenType DoubleArrow = new("=>");
    public static readonly TokenType Semicolon = new(";");

    public static readonly TokenType SingleArrow = new("->");
    public static readonly TokenType Question = new("?");
    public static readonly TokenType Colon = new(":");
    public static readonly TokenType ColonColon = new("::");

    public static readonly TokenType LeftParen = new("(");
    public static readonly TokenType RightParen = new(")");
    public static readonly TokenType LeftBracket = new("[");
    public static readonly TokenType RightBracket = new("]");
    public static readonly TokenType LeftCurly = new("{");
    public static readonly TokenType RightCurly = new("}");

    // predefined object literal keywords
    public static readonly TokenType True = new("true");
    public static readonly TokenType False = new("false");
    public static readonly TokenType Null = new("null");
    public static readonly TokenType This = new("this");
    public static readonly TokenType Super = new("super");

    public static readonly TokenType Var = new("var");
    public static readonly TokenType Const = new("const");
    public static readonly TokenType Property = new("property");
    public static readonly TokenType Function = new("function");
    public static readonly TokenType Typeof = new("typeof");
    public static readonly TokenType From = new("from");
    public static readonly TokenType Import = new("import");
    public static readonly TokenType Export = new("export");
    public static readonly TokenType Ref = new("ref");
    public static readonly TokenType Operator = new("operator");

    public static readonly TokenType Constructor = new("constructor");
    public static readonly TokenType Deconstructor = new("deconstructor");

    public static readonly TokenType Public = new("public");
    public static readonly TokenType Private = new("private");
    public static readonly TokenType Protected = new("protected");
    public static readonly TokenType Sealed = new("sealed");
    public static readonly TokenType Static = new("static");
    public static readonly TokenType Global = new("global");

    public static readonly TokenType Async = new("async");
    public static readonly TokenType Await = new("await");
    public static readonly TokenType Class = new("class");
    public static readonly TokenType Struct = new("struct");
    public static readonly TokenType Interface = new("interface");
    public static readonly TokenType Enum = new("enum");

    public static readonly TokenType Override = new("override");

    // control flow keywords
    public static readonly TokenType If = new("if");
    public static readonly TokenType Else = new("else");
    public static readonly TokenType Switch = new("switch");
    public static readonly TokenType Match = new("match");
    public static readonly TokenType For = new("for");
    public static readonly TokenType Foreach = new("foreach");
    public static readonly TokenType Do = new("do");
    public static readonly TokenType While = new("while");
    public static readonly TokenType Try = new("try");
    public static readonly TokenType Catch = new("catch");
    public static readonly TokenType Finally = new("finally");
    public static readonly TokenType Break = new("break");
    public static readonly TokenType Continue = new("continue");
    public static readonly TokenType Return = new("return");
    public static readonly TokenType Throw = new("throw");
    public static readonly TokenType Yield = new("yield");

    // operator keywords
    public static readonly TokenType In = new("in");
    public static readonly TokenType NotIn = new("!in");
    public static readonly TokenType Is = new("is");
    public static readonly TokenType IsNot = new("!is");
    public static readonly TokenType As = new("as");
    public static readonly TokenType NullableAs = new("?as");
}