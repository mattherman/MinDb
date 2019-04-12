using System;
using MinDb.Compiler;
using MinDb.Storage;

namespace MinDb.Core
{
    public class Database
    {
        private readonly CommandProcessor _commandProcessor;

        public Database(string databaseFilename)
        {
            _commandProcessor = new CommandProcessor(databaseFilename);
        }

        public string Execute(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return "Unable to process query: no input";
            }
            
            try
            {
                var queryResult = _commandProcessor.Process(query);
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
            catch (QueryExecutionException ex)
            {
                return $"Failed to execute query: {ex.Message}";
            }
        }
    }
}
