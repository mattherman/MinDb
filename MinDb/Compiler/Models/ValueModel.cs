namespace MinDb.Compiler.Models
{
    internal class ValueModel
    {
        public ValueType Type { get; }
        public string Value { get; }

        public ValueModel(ValueType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            var formattedValue = Type == ValueType.Integer ? Value : "'" + Value + "'";
            return $"{Type} {formattedValue}";
        }
    }

    internal enum ValueType
    {
        String,
        Integer
    }
}