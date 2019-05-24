# MinDb 

A minimal SQLite-like database written in C# in order to learn more about database internals.

Example:
```
db > INSERT INTO Users VALUES (1, 'John', 'Smith', 30), (2, 'Sally', 'Smith', 19)
[DEBUG] Tokens = [InsertKeyword, Whitespace, IntoKeyword, Whitespace, Object 'users', Whitespace, ValuesKeyword, Whitespace, OpenParenthesis, Integer '1', Comma, Whitespace, StringLiteral 'john', Comma, Whitespace, StringLiteral 'smith', Comma, Whitespace, Integer '30', CloseParenthesis, Comma, Whitespace, OpenParenthesis, Integer '2', Comma, Whitespace, StringLiteral 'sally', Comma, Whitespace, StringLiteral 'smith', Comma, Whitespace, Integer '19', CloseParenthesis, EndOfSequence]
[DEBUG] INSERT | Table = users, Values = [(1,'john','smith',30),(2,'sally','smith',19)]

db > SELECT Id, FirstName FROM Users WHERE LastName = 'Smith' AND Age > 25
[DEBUG] Tokens = [SelectKeyword, Whitespace, Object 'id', Comma, Whitespace, Object 'firstname', Whitespace, FromKeyword, Whitespace, Object 'users', Whitespace, WhereKeyword, Whitespace, Object 'lastname', Whitespace, Equal, Whitespace, StringLiteral 'smith', Whitespace, AndKeyword, Whitespace, Object 'age', Whitespace, GreaterThan, Whitespace, Integer '25', EndOfSequence]
[DEBUG] SELECT | Table = users, Columns = [id,firstname], Condition = [And [Equal lastname 'smith'] [GreaterThan age 25]]

db > DELETE FROM Users WHERE Id = 1
[DEBUG] Tokens = [DeleteKeyword, Whitespace, FromKeyword, Whitespace, Object 'users', Whitespace, WhereKeyword, Whitespace, Object 'id', Whitespace, Equal, Whitespace, Integer '1', EndOfSequence]
[DEBUG] DELETE | Table = users, Condition = [Equal id 1]
```