internal enum TokenType
{
    // Default
    Undefined,
    
    // Keywords
    SelectKeyword,
    FromKeyword,
    WhereKeyword,
    InsertKeyword,
    ValuesKeyword,
    AndKeyword,
    OrKeyword,

    // Objects
    Object,
    StringLiteral,
    Integer,

    // Operators
    Equal,
    NotEqual,
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual,
    
    // Punctuation
    Comma,
    Star,
    OpenParenthesis,
    CloseParenthesis,
    Whitespace
}