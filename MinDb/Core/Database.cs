using System;

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
            
            var queryModel = CommandProcessor.Process(_databaseFilename, query);
            return "SUCCESS";
        }
    }
}
