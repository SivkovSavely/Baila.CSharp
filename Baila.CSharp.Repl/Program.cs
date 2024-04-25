using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Lexer;
using Baila.CSharp.Parser;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Visitors;

IValue? Evaluate(string sourceCode, string filename, bool showTokens = false, bool showAst = false)
{
    var lexer = new Lexer(sourceCode, filename);
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
    
    new FunctionDefiningVisitor().VisitStatements(ast);
    
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

    return ast.LastEvaluatedValue;
}

if (args.Length > 0)
{
    var filename = args[0];
    Evaluate(File.ReadAllText(filename), filename);
    return;
}

Console.WriteLine("Hello from Baila");

var showTokens = false;
var showAst = false;

while (true)
{
    try
    {
        Console.Write("> ");
        var source = Console.ReadLine() ?? "";

        switch (source)
        {
            case "#tokens":
                showTokens = !showTokens;
                Console.WriteLine($"Now {(showTokens ? "" : "not ")}showing tokens.");
                break;
            case "#ast":
                showAst = !showAst;
                Console.WriteLine($"Now {(showAst ? "" : "not ")}showing AST.");
                break;
            case "#nametable":
            {
                Console.WriteLine("Nametable:");
                foreach (var member in NameTable.GetAllMembers())
                {
                    Console.WriteLine($"  {member.Name} : {member.Type} = {member.Value}");
                }

                break;
            }
        }

        var lastEvaluatedValue = Evaluate(source, "<REPL>");

        if (lastEvaluatedValue != null)
        {
            Console.WriteLine(lastEvaluatedValue.GetAsString());
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