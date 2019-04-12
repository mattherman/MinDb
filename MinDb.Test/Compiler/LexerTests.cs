using System;
using Xunit;
using MinDb.Compiler;
using MinDb.Compiler.Models;
using System.Linq;
using System.Collections.Generic;

namespace MinDb.Test
{
    public class LexerTests
    {
        private IList<Token> Tokenize(string query)
        {
            return new Lexer(query).Tokenize().ToList();
        }

        [Fact]
        public void Tokenize_SelectKeyword()
        {
            var tokens = Tokenize("SELECT");
            AssertTokenSequence(tokens, TokenType.SelectKeyword, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_FromKeyword()
        {
            var tokens = Tokenize("FROM");
            AssertTokenSequence(tokens, TokenType.FromKeyword, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_WhereKeyword()
        {
            var tokens = Tokenize("WHERE");
            AssertTokenSequence(tokens, TokenType.WhereKeyword, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_InsertKeyword()
        {
            var tokens = Tokenize("INSERT");
            AssertTokenSequence(tokens, TokenType.InsertKeyword, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_IntoKeyword()
        {
            var tokens = Tokenize("INTO");
            AssertTokenSequence(tokens, TokenType.IntoKeyword, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_ValuesKeyword()
        {
            var tokens = Tokenize("VALUES");
            AssertTokenSequence(tokens, TokenType.ValuesKeyword, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_DeleteKeyword()
        {
            var tokens = Tokenize("DELETE");
            AssertTokenSequence(tokens, TokenType.DeleteKeyword, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_AndKeyword()
        {
            var tokens = Tokenize("AND");
            AssertTokenSequence(tokens, TokenType.AndKeyword, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_OrKeyword()
        {
            var tokens = Tokenize("OR");
            AssertTokenSequence(tokens, TokenType.OrKeyword, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_Star()
        {
            var tokens = Tokenize("*");
            AssertTokenSequence(tokens, TokenType.Star, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_Object()
        {
            var tokens = Tokenize("Test");
            AssertTokenSequence(tokens, TokenType.Object, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_Object_WithLettersAndDigits()
        {
            var tokens = Tokenize("Test123");
            AssertTokenSequence(tokens, TokenType.Object, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_Integer()
        {
            var tokens = Tokenize("123456");
            AssertTokenSequence(tokens, TokenType.Integer, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_Integer_ExceedsMaxSize()
        {
            Assert.Throws<LexerException>(() => Tokenize("123456789123456789123456789"));
        }

        [Fact]
        public void Tokenize_Integer_WithLettersAndDigits()
        {
            Assert.Throws<LexerException>(() => Tokenize("123aBC"));
        }

        [Fact]
        public void Tokenize_StringLiteral()
        {
            var tokens = Tokenize("'hello'");
            AssertTokenSequence(tokens, TokenType.StringLiteral, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_StringLiteral_MultiTerm()
        {
            var tokens = Tokenize("'hello world'");
            AssertTokenSequence(tokens, TokenType.StringLiteral, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_StringLiteral_NeverClosed()
        {
            Assert.Throws<LexerException>(() => Tokenize("'test a multiline unclosed string"));
        }

        [Fact]
        public void Tokenize_Operator_Equal()
        {
            var tokens = Tokenize("=");
            AssertTokenSequence(tokens, TokenType.Equal, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_Operator_NotEqual()
        {
            var tokens = Tokenize("<>");
            AssertTokenSequence(tokens, TokenType.NotEqual, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_Operator_LessThan()
        {
            var tokens = Tokenize("<");
            AssertTokenSequence(tokens, TokenType.LessThan, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_Operator_LessThanOrEqual()
        {
            var tokens = Tokenize("<=");
            AssertTokenSequence(tokens, TokenType.LessThanOrEqual, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_Operator_GreaterThan()
        {
            var tokens = Tokenize(">");
            AssertTokenSequence(tokens, TokenType.GreaterThan, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_Operator_GreaterThanOrEqual()
        {
            var tokens = Tokenize(">=");
            AssertTokenSequence(tokens, TokenType.GreaterThanOrEqual, TokenType.EndOfSequence);
        }

        [Fact]
        public void Tokenize_SelectStatement()
        {
            const string statement = "SELECT * FROM Users WHERE (FirstName = 'Matt' AND LastName = 'Herman') OR Age > 0";
            var tokens = Tokenize(statement);

            AssertTokenSequence(
                tokens,
                TokenType.SelectKeyword,
                TokenType.Whitespace,
                TokenType.Star,
                TokenType.Whitespace,
                TokenType.FromKeyword,
                TokenType.Whitespace,
                TokenType.Object,
                TokenType.Whitespace,
                TokenType.WhereKeyword,
                TokenType.Whitespace,
                TokenType.OpenParenthesis,
                TokenType.Object,
                TokenType.Whitespace,
                TokenType.Equal,
                TokenType.Whitespace,
                TokenType.StringLiteral,
                TokenType.Whitespace,
                TokenType.AndKeyword,
                TokenType.Whitespace,
                TokenType.Object,
                TokenType.Whitespace,
                TokenType.Equal,
                TokenType.Whitespace,
                TokenType.StringLiteral,
                TokenType.CloseParenthesis,
                TokenType.Whitespace,
                TokenType.OrKeyword,
                TokenType.Whitespace,
                TokenType.Object,
                TokenType.Whitespace,
                TokenType.GreaterThan,
                TokenType.Whitespace,
                TokenType.Integer,
                TokenType.EndOfSequence
            );
        }

        [Fact]
        public void Tokenize_InsertStatement()
        {
            const string statement = "INSERT INTO Users VALUES (1, 'John'), (2, 'Sally')";
            var tokens = Tokenize(statement);

            AssertTokenSequence(
                tokens,
                TokenType.InsertKeyword,
                TokenType.Whitespace,
                TokenType.IntoKeyword,
                TokenType.Whitespace,
                TokenType.Object,
                TokenType.Whitespace,
                TokenType.ValuesKeyword,
                TokenType.Whitespace,
                TokenType.OpenParenthesis,
                TokenType.Integer,
                TokenType.Comma,
                TokenType.Whitespace,
                TokenType.StringLiteral,
                TokenType.CloseParenthesis,
                TokenType.Comma,
                TokenType.Whitespace,
                TokenType.OpenParenthesis,
                TokenType.Integer,
                TokenType.Comma,
                TokenType.Whitespace,
                TokenType.StringLiteral,
                TokenType.CloseParenthesis,
                TokenType.EndOfSequence
            );
        }

        [Fact]
        public void Tokenize_DeleteStatement()
        {
            const string statement = "DELETE FROM Users";
            var tokens = Tokenize(statement);

            AssertTokenSequence(
                tokens,
                TokenType.DeleteKeyword,
                TokenType.Whitespace,
                TokenType.FromKeyword,
                TokenType.Whitespace,
                TokenType.Object,
                TokenType.EndOfSequence
            );
        }

        private void AssertTokenSequence(IList<Token> tokens, params TokenType[] expectedTokens)
        {
            Assert.Equal(expectedTokens.Length, tokens.Count);
            for (var i = 0; i < tokens.Count; i++)
            {
                Assert.Equal(expectedTokens[i], tokens[i].Type);
            }
        }
    }
}
