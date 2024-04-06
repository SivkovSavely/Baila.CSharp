using Baila.CSharp.Lexer;

Console.WriteLine("Hello from Baila");

while (true)
{
    Console.Write("> ");
    var source = Console.ReadLine() ?? "";
    var lexer = new Lexer(source, "<REPL>");
    var tokens = lexer.Tokenize();
    foreach (var token in tokens)
    {
        Console.WriteLine(token);
    }
}