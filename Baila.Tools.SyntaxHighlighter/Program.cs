using Baila.CSharp.Lexer;

if (args.Length >= 1)
{
    var path = Path.GetFullPath(args[0]);

    if (!Directory.Exists(path))
    {
        Console.WriteLine($"Folder {path} doesn't exist");
        return 1;
    }

    var files = Directory.GetFiles(path);
    foreach (var file in files)
    {
        HighlightFile(file);
    }
}

return 0;

void HighlightFile(string filePath)
{
    var source = File.ReadAllText(filePath);
    var lexer = new Lexer(source, filePath);
    var tokens = lexer.Tokenize();
}