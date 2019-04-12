using System.Collections.Generic;

namespace MinDb.Compiler.Models
{
    internal class ValueModelGroup
    {
        private IEnumerable<ValueModel> _values;
        public IEnumerable<ValueModel> Values
        {
            get { return _values != null ? _values : new List<ValueModel>(); }
            set { _values = value; }
        }

        public override string ToString()
        {
            return $"({string.Join(",", Values)})";
        }
    }
}