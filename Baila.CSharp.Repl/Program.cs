using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Lexer;
using Baila.CSharp.Parser;

if (args.Length > 0)
{
    var lexer = new Lexer(File.ReadAllText(args[0]), args[0]);
    var parser = new Parser(lexer.Tokenize());
    var ast = parser.BuildAst();
    ast.Execute();
    return;
}

Console.WriteLine("Hello from Baila");

bool showTokens = false;
bool showAst = false;

while (true)
{
    try
    {
        Console.Write("> ");
        var source = Console.ReadLine() ?? "";

        if (source == "#tokens")
        {
            showTokens = !showTokens;
            Console.WriteLine($"Now {(showTokens ? "" : "not ")}showing tokens.");
        }
        else if (source == "#ast")
        {
            showAst = !showAst;
            Console.WriteLine($"Now {(showAst ? "" : "not ")}showing AST.");
        }
        else if (source == "#nametable")
        {
            Console.WriteLine("Nametable:");
            foreach (var member in NameTable.GetAllMembers())
            {
                Console.WriteLine($"  {member.Name} : {member.Type} = {member.Value}");
            }
        }
        
        var lexer = new Lexer(source, "<REPL>");
        var tokens = lexer.Tokenize();

#if DEBUG
        if (showTokens)
        {
            Console.WriteLine("TOKENS:");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            foreach (var token in tokens)
            {
                Console.WriteLine($"  {token}");
            }

            Console.ResetColor();
        }
#endif

        var parser = new Parser(tokens);
        var ast = parser.BuildAst();

#if DEBUG
        if (showAst)
        {
            Console.WriteLine("PROGRAM AST:");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"  {ast}");
            Console.ResetColor();
        }
#endif
        ast.Execute();

        if (ast.LastEvaluatedValue is { } value)
        {
            Console.WriteLine(value.GetAsString());
        }
    }
    catch (Exception e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(e.Message);
        if (e.StackTrace is { } st) Console.WriteLine(string.Join("\n", st.Split("\n").Take(2)));
        Console.ResetColor();
    }
}