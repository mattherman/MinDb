using System;

namespace MinDb.Storage
{
    internal class QueryExecutionException : Exception
    {
        public QueryExecutionException(string message) : base(message) { }
    }

    internal class DataAccessException : QueryExecutionException
    {
        public DataAccessException(string message) : base(message) { }
    }
}