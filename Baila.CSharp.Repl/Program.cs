using Baila.CSharp.Interpreter.Stdlib;
using Baila.CSharp.Repl;

if (args.Length > 0)
{
    var filename = args[0];
    Interpreter.Evaluate(File.ReadAllText(filename), filename);
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

        var lastEvaluatedValue = Interpreter.Evaluate(source, "<REPL>");

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