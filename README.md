# MinDb 

A minimal SQLite-like database written in C# in order to learn more about database internals.

Example:
```
db > INSERT INTO Users VALUES (1, 'John'), (2, 'Sally')
[DEBUG] Tokens = [InsertKeyword, Whitespace, IntoKeyword, Whitespace, Object 'users', Whitespace, ValuesKeyword, Whitespace, OpenParenthesis, Integer '1', Comma, Whitespace, StringLiteral 'john', CloseParenthesis, Comma, Whitespace, OpenParenthesis, Integer '2', Comma, Whitespace, StringLiteral 'sally', CloseParenthesis, EndOfSequence]
[DEBUG] INSERT | Table = users Values = [(Integer 1,String 'john'),(Integer 2,String 'sally')]
Success.
db > SELECT Id, Name FROM Users
[DEBUG] Tokens = [SelectKeyword, Whitespace, Object 'id', Comma, Whitespace, Object 'name', Whitespace, FromKeyword, Whitespace, Object 'users', EndOfSequence]
[DEBUG] SELECT | Table = users, Columns = [id,name]
Success.
db > DELETE FROM Users
[DEBUG] Tokens = [DeleteKeyword, Whitespace, FromKeyword, Whitespace, Object 'users', EndOfSequence]
[DEBUG] DELETE | Table = users
Success.
```