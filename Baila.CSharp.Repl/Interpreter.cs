using Baila.CSharp.Ast.Diagnostics;
using Baila.CSharp.Ast.Syntax.Statements;
using Baila.CSharp.Runtime.Values.Abstractions;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Repl;

public static class Interpreter
{
    public static IValue? Evaluate(string sourceCode, string filename, bool showTokens = false, bool showAst = false)
    {
        try
        {
            var ast = Compile(sourceCode, filename, showTokens, showAst);
            ast.Execute();
            return ast.LastEvaluatedValue;
        }
        catch (ParseException e)
        {
            foreach (var diagnostic in e.Diagnostics)
            {
                var previousColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;

                Console.Write("Syntax error: ");
                Console.WriteLine(diagnostic.GetErrorMessage());

                Console.ForegroundColor = previousColor;

                var maxLines = sourceCode.Split("\n").Length.ToString().Length;

                foreach (var diagnosticLineSpan in diagnostic.GetLines())
                {
                    var extraSpaces = maxLines - diagnosticLineSpan.LineNumber.ToString().Length;

                    var lineOffset = 2 + maxLines + 3;

                    Console.Write("  ");
                    Console.Write(new string(' ', extraSpaces));
                    Console.Write(diagnosticLineSpan.LineNumber);
                    Console.Write(" | ");
                    Console.WriteLine(diagnosticLineSpan.FullLine);

                    Console.Write(new string(' ', lineOffset));
                    Console.Write(new string(' ', diagnosticLineSpan.StartColumn - 1));
                    Console.WriteLine(new string('^', diagnosticLineSpan.Length));
                }
            }
        }

        return null;
    }

    public static Statements Compile(string sourceCode, string filename, bool showTokens = false, bool showAst = false)
    {
        var lexer = new Lexer.Lexer(sourceCode, filename);
        var tokens = lexer.Tokenize();

        if (lexer.Diagnostics.Any())
        {
            throw new ParseException(lexer.Diagnostics);
        }

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