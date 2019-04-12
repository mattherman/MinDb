using System.Collections.Generic;

namespace MinDb.Compiler.Models
{
    internal class ValueModelGroup
    {
        public IEnumerable<ValueModel> ColumnValues { get; }

        public ValueModelGroup()
        {
            ColumnValues = new List<ValueModel>();
        }

        public override string ToString()
        {
            return $"({string.Join(",", ColumnValues)})";
        }
    }
}