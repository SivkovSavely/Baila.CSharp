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

                Console.WriteLine();
                Console.Write($"[{diagnostic.GetCode()}] Syntax error: ");
                Console.WriteLine(diagnostic.GetErrorMessage());

                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine(diagnostic.GetFilename());

                Console.ForegroundColor = previousColor;

                var maxLines = sourceCode.Split("\n").Length.ToString().Length;

                foreach (var diagnosticLineSpan in diagnostic.GetLines())
                {
                    var extraSpaces = maxLines - diagnosticLineSpan.LineNumber.ToString().Length;

                    var lineOffset = 2 + maxLines;

                    Console.Write("  ");
                    Console.Write(new string(' ', extraSpaces));
                    Console.Write(diagnosticLineSpan.LineNumber);
                    Console.Write(" | ");
                    Console.WriteLine(diagnosticLineSpan.FullLine);

                    Console.Write(new string(' ', lineOffset));
                    Console.Write(" | ");
                    Console.Write(new string(' ', diagnosticLineSpan.StartColumn - 1));
                    Console.Write(new string('^', diagnosticLineSpan.Length));
                    
                    if (diagnosticLineSpan.Length == 1)
                    {
                        Console.WriteLine(
                            $"-- {diagnosticLineSpan.StartColumn}");
                    }
                    else
                    {
                        Console.WriteLine(
                            $"-- {diagnosticLineSpan.StartColumn}..{diagnosticLineSpan.StartColumn + diagnosticLineSpan.Length - 1}");
                    }
                }
            }
        }

        return null;
    }

    public static Statements Compile(string sourceCode, string filename, bool showTokens = false, bool showAst = false)
    {
        sourceCode = sourceCode
            .Replace("\r\n", "\n")
            .Replace("\r", "\n");

        var sourceLines = sourceCode.Split("\n");
        
        var lexer = new Lexer.Lexer(
            sourceCode,
            filename);
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

        var parser = new Parser.Parser(filename, sourceCode, tokens);
        var ast = parser.BuildAst();

        if (parser.Diagnostics.Any())
        {
            throw new ParseException(parser.Diagnostics);
        }

        var typeCheckingVisitor = new TypeCheckingVisitor([], sourceLines);
        typeCheckingVisitor.VisitStatements(ast);

        if (typeCheckingVisitor.Diagnostics.Any())
        {
            throw new ParseException(typeCheckingVisitor.Diagnostics);
        }

        new RuntimeFunctionDefiningVisitor().VisitStatements(ast);

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