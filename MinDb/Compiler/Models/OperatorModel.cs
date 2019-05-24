namespace MinDb.Compiler.Models
{
    internal class OperatorModel : IConditionNode
    {
        public OperatorType Type { get; }

        public OperatorModel(OperatorType type)
        {
            Type = type;
        }

        public override string ToString()
        {
            return Type.ToString();
        }
    }

    internal enum OperatorType
    {
        Unknown,
        Equal,
        NotEqual,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        And,
        Or
    }
}