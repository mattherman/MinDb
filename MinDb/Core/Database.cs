using System;

namespace MinDb.Core
{
    public class Database
    {
        private readonly string _databaseFilename;
        private readonly CommandProcessor _processor;

        public Database(string databaseFilename)
        {
            _databaseFilename = databaseFilename;
            _processor = new CommandProcessor();
        }

        public string Execute(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return "Unable to process query: no input";
            }
            
            return _processor.Process(_databaseFilename, query);
        }
    }
}
