using System;
using Xunit;
using MinDb.Compiler;
using System.Linq;

public class ParserTests
{
    private T Parse<T>(params Token[] tokens) where T : QueryModel
    {
        var model = new Parser(tokens).Parse();
        return (T)model;
    }

    [Fact]
    public void Parse_SelectWithColumns()
    {
        var model = Parse<SelectQueryModel>(
            new Token(TokenType.SelectKeyword, null),
            new Token(TokenType.Object, "FirstName"),
            new Token(TokenType.Comma, null),
            new Token(TokenType.Object, "LastName"),
            new Token(TokenType.FromKeyword, null),
            new Token(TokenType.Object, "Users")
        );

        var columns = model.TargetColumns.ToList();
        Assert.Equal("FirstName", columns[0]);
        Assert.Equal("LastName", columns[1]);

        Assert.Equal("Users", model.TargetTable);
    }

    [Fact]
    public void Parse_Select_ColumnListMustFollowSelect()
    {
        Assert.Throws<ParserException>(() =>
            Parse<SelectQueryModel>(
                new Token(TokenType.SelectKeyword, null),
                new Token(TokenType.FromKeyword, null)
            )
        );
    }

    [Fact]
    public void Parse_Select_FromMustFollowColumnList()
    {
        Assert.Throws<ParserException>(() =>
            Parse<SelectQueryModel>(
                new Token(TokenType.SelectKeyword, null),
                new Token(TokenType.Object, "FirstName")
            )
        );
    }

    [Fact]
    public void Parse_Select_TableMustFollowFrom()
    {
        Assert.Throws<ParserException>(() =>
            Parse<SelectQueryModel>(
                new Token(TokenType.SelectKeyword, null),
                new Token(TokenType.Object, "FirstName"),
                new Token(TokenType.FromKeyword, null),
                new Token(TokenType.WhereKeyword, null)
            )
        );
    }

    [Fact]
    public void Parse_Insert()
    {
        var model = Parse<InsertQueryModel>(
            new Token(TokenType.InsertKeyword, null),
            new Token(TokenType.IntoKeyword, null),
            new Token(TokenType.Object, "Users"),
            new Token(TokenType.ValuesKeyword, null),
            new Token(TokenType.OpenParenthesis, null),
            new Token(TokenType.Integer, "1"),
            new Token(TokenType.Comma, null),
            new Token(TokenType.StringLiteral, "John"),
            new Token(TokenType.CloseParenthesis, null),
            new Token(TokenType.Comma, null),
            new Token(TokenType.OpenParenthesis, null),
            new Token(TokenType.Integer, "2"),
            new Token(TokenType.Comma, null),
            new Token(TokenType.StringLiteral, "Jame"),
            new Token(TokenType.CloseParenthesis, null)
        );

        Assert.Equal("Users", model.TargetTable);
    }

    [Fact]
    public void Parse_Delete()
    {
        var model = Parse<DeleteQueryModel>(
            new Token(TokenType.DeleteKeyword, null),
            new Token(TokenType.FromKeyword, null),
            new Token(TokenType.Object, "Users")
        );

        Assert.Equal("Users", model.TargetTable);
    }
}