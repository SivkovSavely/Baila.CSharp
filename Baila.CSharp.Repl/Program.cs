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

while (true)
{
    try
    {
        Console.Write("> ");
        var source = Console.ReadLine() ?? "";
        var lexer = new Lexer(source, "<REPL>");
        var tokens = lexer.Tokenize();

#if DEBUG
        Console.WriteLine("TOKENS:");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
        Console.ResetColor();
#endif

        var parser = new Parser(tokens);
        var ast = parser.BuildAst();

#if DEBUG
        Console.WriteLine("PROGRAM AST:");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(ast);
        Console.ResetColor();

        Console.WriteLine("PROGRAM RUNNING:");
#endif
        ast.Execute();
    }
    catch (Exception e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(e);
        Console.ResetColor();
    }
}