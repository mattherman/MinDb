# MinDb 

A minimal SQLite-like database written in C# in order to learn more about database internals.

Example:
```
db > SELECT Id, Name, Age FROM Users
[DEBUG] Tokens = [SelectKeyword, Whitespace, Object 'id', Comma, Whitespace, Object 'name', Comma, Whitespace, Object 'age', Whitespace, FromKeyword, Whitespace, Object 'users', EndOfSequence]
[DEBUG] SELECT | Table = users, Columns = [id,name,age]
Success.
```