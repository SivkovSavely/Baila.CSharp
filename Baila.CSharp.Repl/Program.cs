using Baila.CSharp.Interpreter.StatementInterpreters;
using Baila.CSharp.Lexer;
using Baila.CSharp.Parser;

Console.WriteLine("Hello from Baila");

while (true)
{
    try
    {
        Console.Write("> ");
        var source = Console.ReadLine() ?? "";
        var lexer = new Lexer(source, "<REPL>");
        var tokens = lexer.Tokenize();

        Console.WriteLine("TOKENS:");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
        Console.ResetColor();

        var parser = new Parser(tokens);
        var ast = parser.BuildAst();
        Console.WriteLine("PROGRAM AST:");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(ast);
        Console.ResetColor();

        Console.WriteLine("PROGRAM RUNNING:");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        ast.InterpretExecute();
        Console.ResetColor();
    }
    catch (Exception e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(e);
        Console.ResetColor();
    }
}