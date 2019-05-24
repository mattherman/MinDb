namespace MinDb.Compiler.Models
{
    internal class ValueModel : IConditionNode
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
            return Type == ValueType.Integer ? Value : "'" + Value + "'";
        }
    }

    internal enum ValueType
    {
        String,
        Integer
    }
}