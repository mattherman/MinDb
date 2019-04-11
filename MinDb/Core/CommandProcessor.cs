using System;
using System.Linq;
using MinDb.Compiler;

internal static class CommandProcessor
{
    public static QueryModel Process(string databaseFilename, string query)
    {
        var lexer = new Lexer(query);
        var tokens = lexer.Tokenize();
        
        Console.WriteLine($"[DEBUG] {string.Join(string.Empty, tokens.Select(t => t.Value))}");

        var parser = new Parser(tokens);
        var queryModel = parser.Parse();

        Console.WriteLine($"[DEBUG] Table = {queryModel.TargetTable}, Columns = [{string.Join(",", queryModel.TargetColumns)}]");

        return queryModel;
    }
}