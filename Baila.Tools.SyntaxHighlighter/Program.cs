// https://codesandbox.io/p/sandbox/naughty-wu-yh75d2

using System.Text;
using Baila.CSharp.Ast.Lexer;

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
    var source = File.ReadAllText(filePath).Replace("\r\n", "\n");
    var lexer = new Lexer(source, filePath, LexerMode.Highlighting);
    var tokens = lexer.Tokenize();

    var html = new StringBuilder();
    html.Append("<pre>");

    for (var index = 0; index < tokens.Count; index++)
    {
        var token = tokens[index];
        AppendToken(token, ref index);
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
                    
                    .unknown { color: black; text-decoration: underline; }
                    .comment { color: gray; }
                    .keyword { color: #d73a49; }
                    .string { color: #22863a; }
                    .number { color: #316bcd; }
                    .keyword { color: #e36209; }
                    .operator { color: #24292e; }
                    </style>
                    """);

    var htmlFilePath = Path.Combine(
        Path.GetDirectoryName(filePath)!,
        Path.GetFileNameWithoutExtension(filePath) + ".html");

    File.WriteAllText(htmlFilePath, html.ToString());

    return;

    void AppendToken(Token token, ref int index)
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
        else if (token.Type == TokenType.SingleQuoteStringLiteral)
        {
            AppendSpan("string", "'" + token.Value + "'");
        }
        else if (token.Type == TokenType.DoubleQuoteStringLiteral)
        {
            AppendSpan("string", "\"" + token.Value + "\"");
        }
        else if (token.Type == TokenType.BacktickStringLiteral)
        {
            AppendSpan("string", "`" + token.Value + "`");
        }
        else if (token.Type == TokenType.NumberLiteral)
        {
            AppendSpan("number", token.Value!);
        }
        else if (token.Type.IsKeyword)
        {
            AppendSpan("keyword", token.Type.Type);
        }
        else if (token.Type.IsOperator)
        {
            AppendSpan("operator", token.Type.Type);
        }
        else if (token.Type == TokenType.PrivateStringConcat)
        {
            bool? isSingleQuote = null;

            if (tokens[index + 1].Type != TokenType.LeftParen)
            {
                throw new Exception("Token after [[string_concat]] should be '('");
            }

            index++; // skip [[string_concat]]
            index++; // skip (
            
            token = tokens[index];
            if (token.Type == TokenType.SingleQuoteStringLiteral)
            {
                isSingleQuote = true;
                AppendSpan("string", "'");
            }
            else if (token.Type == TokenType.DoubleQuoteStringLiteral)
            {
                isSingleQuote = false;
                AppendSpan("string", "\"");
            }
            else
            {
                throw new Exception("isSingleQuote is null");
            }

            do
            {
                token = tokens[index];

                if (token.Type == TokenType.SingleQuoteStringLiteral)
                {
                    AppendSpan("string", token.Value!);
                    index++;
                    token = tokens[index];
                }
                else if (token.Type == TokenType.DoubleQuoteStringLiteral)
                {
                    AppendSpan("string", token.Value!);
                    index++;
                    token = tokens[index];
                }
                else if (token.Type == TokenType.Comma)
                {
                    index++;  // skip comma
                    token = tokens[index];
                } else if (token.Type == TokenType.LeftParen)
                {
                    index++; // skip (
                    AppendSpan("operator", "${");

                    do
                    {
                        token = tokens[index];
                        AppendToken(token, ref index);
                        index++;
                        token = tokens[index];
                    } while (token.Type != TokenType.RightParen);
                    AppendSpan("operator", "}");
                    
                    index++; // skip )
                    token = tokens[index];
                }
                
            } while (token.Type != TokenType.RightParen);

            // skipping ) is provided by the end of the for loop
            if (isSingleQuote == true)
            {
                AppendSpan("string", "'");
            }
            else if (isSingleQuote == false)
            {
                AppendSpan("string", "\"");
            }
            else
            {
                throw new Exception("isSingleQuote is null");
            }
            return;
        }
        else
        {
            AppendSpan("unknown", token.Value ?? token.Type.Type);
        }
    }

    void AppendSpan(string className, string contents)
    {
        html.Append($"""
                     <span class="{className}">{contents}</span>
                     """);
    }
}