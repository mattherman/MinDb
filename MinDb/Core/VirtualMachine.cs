using System;
using MinDb.Compiler.Models;
using MinDb.Storage;

namespace MinDb.Core
{
    internal class VirtualMachine
    {
        private readonly Store _store;

        public VirtualMachine(string databaseFilename)
        {
            _store = new Store(databaseFilename);
        }

        public QueryResult Execute(QueryModel query)
        {
            if (query is SelectQueryModel)
                return _store.Select(query as SelectQueryModel);
            else if (query is InsertQueryModel)
                return _store.Insert(query as InsertQueryModel);
            else if (query is DeleteQueryModel)
                return _store.Delete(query as DeleteQueryModel);
            else 
                throw new QueryExecutionException("Unknown query type");
        }
    }
}