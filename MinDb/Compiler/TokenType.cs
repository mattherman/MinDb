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
    DeleteKeyword,
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