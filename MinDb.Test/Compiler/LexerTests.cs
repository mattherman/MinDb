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
            AssertTokenSequence(tokens, TokenType.SelectKeyword);
        }

        [Fact]
        public void Tokenize_FromKeyword()
        {
            var tokens = Tokenize("FROM");
            AssertTokenSequence(tokens, TokenType.FromKeyword);
        }

        [Fact]
        public void Tokenize_WhereKeyword()
        {
            var tokens = Tokenize("WHERE");
            AssertTokenSequence(tokens, TokenType.WhereKeyword);
        }

        [Fact]
        public void Tokenize_InsertKeyword()
        {
            var tokens = Tokenize("INSERT");
            AssertTokenSequence(tokens, TokenType.InsertKeyword);
        }

        [Fact]
        public void Tokenize_ValuesKeyword()
        {
            var tokens = Tokenize("VALUES");
            AssertTokenSequence(tokens, TokenType.ValuesKeyword);
        }

        [Fact]
        public void Tokenize_DeleteKeyword()
        {
            var tokens = Tokenize("DELETE");
            AssertTokenSequence(tokens, TokenType.DeleteKeyword);
        }

        [Fact]
        public void Tokenize_AndKeyword()
        {
            var tokens = Tokenize("AND");
            AssertTokenSequence(tokens, TokenType.AndKeyword);
        }

        [Fact]
        public void Tokenize_OrKeyword()
        {
            var tokens = Tokenize("OR");
            AssertTokenSequence(tokens, TokenType.OrKeyword);
        }

        [Fact]
        public void Tokenize_Star()
        {
            var tokens = Tokenize("*");
            AssertTokenSequence(tokens, TokenType.Star);
        }

        [Fact]
        public void Tokenize_Object()
        {
            var tokens = Tokenize("Test");
            AssertTokenSequence(tokens, TokenType.Object);
        }

        [Fact]
        public void Tokenize_Object_WithLettersAndDigits()
        {
            var tokens = Tokenize("Test123");
            AssertTokenSequence(tokens, TokenType.Object);
        }

        [Fact]
        public void Tokenize_Integer()
        {
            var tokens = Tokenize("123456");
            AssertTokenSequence(tokens, TokenType.Integer);
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
            AssertTokenSequence(tokens, TokenType.StringLiteral);
        }

        [Fact]
        public void Tokenize_StringLiteral_MultiTerm()
        {
            var tokens = Tokenize("'hello world'");
            AssertTokenSequence(tokens, TokenType.StringLiteral);
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
            AssertTokenSequence(tokens, TokenType.Equal);
        }

        [Fact]
        public void Tokenize_Operator_NotEqual()
        {
            var tokens = Tokenize("<>");
            AssertTokenSequence(tokens, TokenType.NotEqual);
        }

        [Fact]
        public void Tokenize_Operator_LessThan()
        {
            var tokens = Tokenize("<");
            AssertTokenSequence(tokens, TokenType.LessThan);
        }

        [Fact]
        public void Tokenize_Operator_LessThanOrEqual()
        {
            var tokens = Tokenize("<=");
            AssertTokenSequence(tokens, TokenType.LessThanOrEqual);
        }

        [Fact]
        public void Tokenize_Operator_GreaterThan()
        {
            var tokens = Tokenize(">");
            AssertTokenSequence(tokens, TokenType.GreaterThan);
        }

        [Fact]
        public void Tokenize_Operator_GreaterThanOrEqual()
        {
            var tokens = Tokenize(">=");
            AssertTokenSequence(tokens, TokenType.GreaterThanOrEqual);
        }

        [Fact]
        public void Tokenize_MultiTokenStatement()
        {
            const string statement = "SELECT * FROM Users WHERE (FirstName = 'Matt' AND LastName = 'Herman') OR Age > 0";
            var tokens = Tokenize(statement);

            Assert.Equal(33, tokens.Count);
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
                TokenType.Integer
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
