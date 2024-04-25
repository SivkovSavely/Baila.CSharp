using Baila.CSharp.Ast.Statements;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Repl;

public static class Interpreter
{
    public static IValue? Evaluate(string sourceCode, string filename, bool showTokens = false, bool showAst = false)
    {
        var ast = Compile(sourceCode, filename, showTokens, showAst);
        ast.Execute();

        return ast.LastEvaluatedValue;
    }

    public static Statements Compile(string sourceCode, string filename, bool showTokens = false, bool showAst = false)
    {
        var lexer = new Lexer.Lexer(sourceCode, filename);
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
    
        var parser = new Parser.Parser(tokens);
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
        return ast;
    }
}