using System.Collections.Generic;

namespace MinDb.Compiler.Models
{
    internal class InsertQueryModel : QueryModel
    {
        private IEnumerable<ValueModelGroup> _rows;
        public IEnumerable<ValueModelGroup> Rows
        {
            get { return _rows != null ? _rows : new List<ValueModelGroup>(); }
            set { _rows = value; }
        }

        public override string ToString()
        {
            return $"INSERT | Table = {TargetTable.Name} Values = [{string.Join(",", Rows)}]";
        }
    }
}