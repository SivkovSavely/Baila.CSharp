using Baila.CSharp.Interpreter.StatementInterpreters;
using Baila.CSharp.Lexer;
using Baila.CSharp.Parser;

Console.WriteLine("Hello from Baila");

while (true)
{
    Console.Write("> ");
    var source = Console.ReadLine() ?? "";
    var lexer = new Lexer(source, "<REPL>");
    var tokens = lexer.Tokenize();

    Console.WriteLine("TOKENS:");
    foreach (var token in tokens)
    {
        Console.WriteLine(token);
    }

    var parser = new Parser(tokens);
    var ast = parser.BuildAst();
    Console.WriteLine("PROGRAM AST:");
    Console.WriteLine(ast);

    Console.WriteLine("PROGRAM RUNNING:");
    ast.InterpretExecute();
}