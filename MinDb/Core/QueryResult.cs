using System.Collections.Generic;

namespace MinDb.Core
{
    public class QueryResult
    {
        private IEnumerable<Row> _rows;
        public IEnumerable<Row> Rows
        {
            get { return _rows != null ? _rows : new List<Row>(); }
            set { _rows = value; }
        }
    }

    public class Row
    {
        
    }
}