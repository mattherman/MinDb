using System.Collections.Generic;

namespace MinDb.Compiler.Models
{
    internal class InsertQueryModel : QueryModel
    {
        public IEnumerable<ValueModelGroup> Rows { get; }

        public InsertQueryModel()
        {
            Rows = new List<ValueModelGroup>();
        }

        public override string ToString()
        {
            return $"INSERT | Table = {TargetTable} Values = [{string.Join(",", Rows)}]";
        }
    }
}