using System.Linq;
using MinDb.Compiler;

internal class CommandProcessor
{
    private readonly Parser _parser;

    public CommandProcessor()
    {
        _parser = new Parser();
    }

    public string Process(string databaseFilename, string query)
    {
        var lexer = new Lexer(query);
        var tokens = lexer.Tokenize();
        return string.Join(string.Empty, tokens.Select(t => t.Value));
    }
}