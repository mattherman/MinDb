using System.Collections.Generic;

namespace MinDb.Compiler.Models
{
    internal class InsertQueryModel : QueryModel
    {
        public IEnumerable<InsertQueryRow> Rows { get; }

        public InsertQueryModel()
        {
            Rows = new List<InsertQueryRow>();
        }

        public override string ToString()
        {
            return $"INSERT | Table = {TargetTable} Values = [{string.Join(",", Rows)}]";
        }
    }

    internal class InsertQueryRow
    {
        public IEnumerable<InsertQueryColumn> Columns { get; }

        public InsertQueryRow()
        {
            Columns = new List<InsertQueryColumn>();
        }

        public override string ToString()
        {
            return $"({string.Join(",", Columns)})";
        }
    }

    internal class InsertQueryColumn
    {
        public InsertQueryColumnType Type { get; }
        public string Value { get; }

        public InsertQueryColumn(InsertQueryColumnType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            var formattedValue = Type == InsertQueryColumnType.Integer ? Value : "'" + Value + "'";
            return $"{Type} {formattedValue}";
        }
    }

    internal enum InsertQueryColumnType
    {
        String,
        Integer
    }
}