using System.Collections.Generic;
using System.Linq;

namespace MinDb.Compiler.Models
{
    internal class SelectQueryModel : QueryModel
    {
        private IEnumerable<ObjectModel> _targetColumns;
        public IEnumerable<ObjectModel> TargetColumns
        {
            get { return _targetColumns != null ? _targetColumns : new List<ObjectModel>(); }
            set { _targetColumns = value; }
        }

        public override string ToString()
        {
            return $"SELECT | Table = {TargetTable.Name}, Columns = [{string.Join(",", TargetColumns.Select(c => c.Name))}]";
        }
    }
}