namespace MinDb.Compiler.Models
{
    internal class Token
    {
        public TokenType Type { get; }
        public string Value { get; }

        public Token(TokenType tokenType, string value)
        {
            Type = tokenType;
            Value = value;
        }

        public override string ToString()
        {
            var valueString = "";
            if (Type == TokenType.Object || Type == TokenType.StringLiteral || Type == TokenType.Integer)
            {
                valueString = $" '{Value}'";
            }
            return $"{Type}{valueString}";
        }
    }
}