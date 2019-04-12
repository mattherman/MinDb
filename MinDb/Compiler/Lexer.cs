using System;
using System.Collections.Generic;
using MinDb.Compiler.Models;

namespace MinDb.Compiler
{
    internal class Lexer
    {
        private readonly string _text;
        private int _position;

        private char Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _text.Length)
            {
                return '\0';
            }

            return _text[index];
        }
        private char Current => Peek(0);
        private char Lookahead => Peek(1);

        private void Next() => Next(1);

        private void Next(int count)
        {
            _position += count;
        }

        public Lexer(string text)
        {
            _text = text.ToLower();
            _position = 0;
        }

        public IEnumerable<Token> Tokenize()
        {
            var tokens = new List<Token>();

            while (_position < _text.Length)
            {
                Token match = null;

                if (Char.IsWhiteSpace(Current))
                {
                    match = Whitespace();
                }
                else if (Char.IsLetter(Current))
                {
                    match = KeywordOrObject();
                }
                else if (Char.IsNumber(Current))
                {
                    match = Number();
                }
                else if (Current == '\'')
                {
                    match = StringLiteral();
                }
                else
                {
                    match = Other();
                }

                if (match == null || match.Type == TokenType.Undefined)
                {
                    throw new LexerException($"Encountered unexpected character: {Current}");
                }

                tokens.Add(new Token(match.Type, match.Value));
            }

            tokens.Add(new Token(TokenType.EndOfSequence, null));

            return tokens;
        }

        private Token Whitespace()
        {
            var value = GetToken(Char.IsWhiteSpace);
            return new Token(TokenType.Whitespace, value);
        }

        private Token KeywordOrObject()
        {
            var value = GetToken((c) => Char.IsNumber(c) || Char.IsLetter(c));

            TokenType tokenType;
            switch (value)
            {
                case "select":
                    tokenType = TokenType.SelectKeyword;
                    break;
                case "from":
                    tokenType = TokenType.FromKeyword;
                    break;
                case "where":
                    tokenType = TokenType.WhereKeyword;
                    break;
                case "insert":
                    tokenType = TokenType.InsertKeyword;
                    break;
                case "into":
                    tokenType = TokenType.IntoKeyword;
                    break;
                case "values":
                    tokenType = TokenType.ValuesKeyword;
                    break;
                case "delete":
                    tokenType = TokenType.DeleteKeyword;
                    break;
                case "and":
                    tokenType = TokenType.AndKeyword;
                    break;
                case "or":
                    tokenType = TokenType.OrKeyword;
                    break;
                case "*":
                    tokenType = TokenType.Star;
                    break;
                default:
                    tokenType = TokenType.Object;
                    break;
            }

            return new Token(tokenType, value);
        }

        private Token Number()
        {
            // Predicate includes letters to that 123abc will be correctly parsed as a single token,
            // but then throw an error due to an invalid integer value.
            var value = GetToken((c) => Char.IsNumber(c) || Char.IsLetter(c));

            var success = int.TryParse(value, out var _);
            if (!success)
            {
                throw new LexerException($"Unable to tokenize integer: {value}");
            }

            return new Token(TokenType.Integer, value);
        }

        private Token StringLiteral()
        {
            // Increments to ignore the opening quote
            Next();
            var value = GetToken((c) => c != '\'');

            if (Current == '\'')
            {
                Next();
            }
            else
            {
                throw new LexerException($"String literal was never closed: {value}");
            }
            
            // Add two to the length to get the correct sequence length with quotes
            return new Token(TokenType.StringLiteral, value);
        }

        private Token Other()
        {
            switch (Current)
            {
                case '=':
                    Next();
                    return new Token(TokenType.Equal, "=");
                case '<':
                    if (Lookahead == '=')
                    {
                        Next(2);
                        return new Token(TokenType.LessThanOrEqual, "<=");
                    }
                    else if (Lookahead == '>')
                    {
                        Next(2);
                        return new Token(TokenType.NotEqual, "<>");
                    }
                    Next();
                    return new Token(TokenType.LessThan, "<");
                case '>':
                    if (Lookahead == '=')
                    {
                        Next(2);
                        return new Token(TokenType.GreaterThanOrEqual, ">=");
                    }
                    Next();
                    return new Token(TokenType.GreaterThan, ">");
                case '(':
                    Next();
                    return new Token(TokenType.OpenParenthesis, "(");
                case ')':
                    Next();
                    return new Token(TokenType.CloseParenthesis, ")");
                case ',':
                    Next();
                    return new Token(TokenType.Comma, ",");
                case '*':
                    Next();
                    return new Token(TokenType.Star, "*");
                default:
                    return new Token(TokenType.Undefined, null);
            }
        }

        // IndexOf will walk the string starting from startIndex until the predicate is no longer true
        private string GetToken(Func<char, bool> predicate)
        {
            var startIndex = _position;
            while (_text.Matches(_position, predicate))
            {
                Next();
            }
            return _text.Substring(startIndex, _position - startIndex);
        }
    }
}
