using System;
using Xunit;
using MinDb.Compiler;
using System.Linq;

public class ParserTests
{
    private QueryModel Parse(params Token[] tokens)
    {
        return new Parser(tokens).Parse();
    }

    [Fact]
    public void Parse_SelectWithColumns()
    {
        var model = Parse(
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
            Parse(
                new Token(TokenType.SelectKeyword, null),
                new Token(TokenType.FromKeyword, null)
            )
        );
    }

    [Fact]
    public void Parse_Select_FromMustFollowColumnList()
    {
        Assert.Throws<ParserException>(() =>
            Parse(
                new Token(TokenType.SelectKeyword, null),
                new Token(TokenType.Object, "FirstName")
            )
        );
    }

    [Fact]
    public void Parse_Select_TableMustFollowFrom()
    {
        Assert.Throws<ParserException>(() =>
            Parse(
                new Token(TokenType.SelectKeyword, null),
                new Token(TokenType.Object, "FirstName"),
                new Token(TokenType.FromKeyword, null),
                new Token(TokenType.WhereKeyword, null)
            )
        );
    }
}