using System;
using MinDb.Compiler;

namespace MinDb.Core
{
    public class Database
    {
        private readonly string _databaseFilename;

        public Database(string databaseFilename)
        {
            _databaseFilename = databaseFilename;
        }

        public string Execute(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return "Unable to process query: no input";
            }
            
            try
            {
                var queryModel = CommandProcessor.Process(_databaseFilename, query);
                return "Success.";
            }
            catch (LexerException ex)
            {
                return $"Failed to tokenize the input: {ex.Message}";
            }
            catch (ParserException ex)
            {
                return $"Failed to parse the input: {ex.Message}";
            }
        }
    }
}
