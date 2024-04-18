using System.Text;
using Baila.CSharp.Lexer;

if (args.Length >= 1)
{
    var path = Path.GetFullPath(args[0]);

    if (!Directory.Exists(path))
    {
        Console.WriteLine($"Folder {path} doesn't exist");
        return 1;
    }

    var files = Directory.GetFiles(path, "*.baila");
    foreach (var file in files)
    {
        HighlightFile(file);
    }
}

return 0;

void HighlightFile(string filePath)
{
    var source = File.ReadAllText(filePath);
    var lexer = new Lexer(source, filePath, LexerMode.Highlighting);
    var tokens = lexer.Tokenize();

    var html = new StringBuilder();
    html.Append("<pre>");

    foreach (var token in tokens)
    {
        if (token.Type == TokenType.Whitespace)
        {
            switch (token.Value)
            {
                case " ":
                    AppendSpan("whitespace", token.Value!);
                    break;
                case "\n":
                    html.Append("<br/>");
                    break;
            }
        }
        else if (token.Type == TokenType.EndOfLine)
        {
            html.Append("<br/>");
        }
        else if (token.Type == TokenType.SingleLineComment)
        {
            AppendSpan("comment", "#" + token.Value);
        }
        else if (token.Type == TokenType.MultiLineComment)
        {
            AppendSpan("comment", "/*" + token.Value + "*/");
        }
        else if (token.Type == TokenType.Identifier)
        {
            AppendSpan("identifier", token.Value!);
        }
        else
        {
            AppendSpan("unknown", token.Value ?? token.Type.Type);
        }
    }

    html.AppendLine("</pre>");

    html.AppendLine("""
                    <style>
                    pre {
                      margin: 1em;
                      padding: 1em;
                      border-radius: 8px;
                      background-color: rgba(0, 0, 0, 0.025);
                      box-shadow: 0 5px 10px 5px rgba(0, 0, 0, 0.2);
                    }
                    
                    .unknown { color: red; font-style: italic; }
                    .comment { color: gray; font-style: italic; }
                    </style>
                    """);

    var htmlFilePath = Path.Combine(
        Path.GetDirectoryName(filePath)!,
        Path.GetFileNameWithoutExtension(filePath) + ".html");

    File.WriteAllText(htmlFilePath, html.ToString());

    return;

    void AppendSpan(string className, string contents)
    {
        html.Append($"""
                     <span class="{className}">{contents}</span>
                     """);
    }
}