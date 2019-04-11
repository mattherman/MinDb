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
        return $"[{Type}]\t {Value}";
    }
}