using MinDb.Compiler;

internal class CommandProcessor
{
    private readonly Lexer _lexer;
    private readonly Parser _parser;

    public CommandProcessor()
    {
        _lexer = new Lexer();
        _parser = new Parser();
    }

    public string Process(string databaseFilename, string query)
    {
        return query;
    }
}