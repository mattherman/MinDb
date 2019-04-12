using System.Collections.Generic;
using System.Linq;

namespace MinDb.Compiler.Models
{
    internal class SelectQueryModel : QueryModel
    {
        public IEnumerable<ObjectModel> TargetColumns { get; set; }

        public override string ToString()
        {
            return $"SELECT | Table = {TargetTable.Name}, Columns = [{string.Join(",", TargetColumns.Select(c => c.Name))}]";
        }
    }
}