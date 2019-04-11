# MinDb 

A minimal SQLite-like database written in C# in order to learn more about database internals.

Example:
```
db > Select FirstName, LastName, Age FROM Users
[DEBUG] select firstname, lastname, age from users
[DEBUG] Table = users, Columns = [firstname,lastname,age]
```