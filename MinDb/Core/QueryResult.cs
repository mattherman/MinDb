using System.Collections.Generic;

namespace MinDb.Core
{
    public class QueryResult
    {
        public IList<Row> Rows { get; }

        public QueryResult()
        {
            Rows = new List<Row>();
        }
    }

    public class Row
    {
        public int Id { get; }
        public string Username { get; }
        public string Email { get; }

        public Row(int id, string username, string email)
        {
            Id = id;
            Username = username;
            Email = email;
        }

        public override string ToString()
        {
            return $"{Id}\t{Username}\t{Email}";
        }
    }
}