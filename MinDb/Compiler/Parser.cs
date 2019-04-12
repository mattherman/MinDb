using System.Collections.Generic;
using System.Linq;
using MinDb.Compiler.Models;

namespace MinDb.Compiler
{
    /*
        == Limited SQL Grammar ==

        S -> SELECT
        S -> INSERT
        S -> DELETE
        
        SELECT -> select TARGET_COLUMNS from object WHERE_CLAUSE

        INSERT -> insert into object values (col, col, col)

        DELETE -> delete from object WHERE_CLAUSE

        INSERT_ROW -> GROUPED_OBJECT_LIST INSERT_ROW_NEXT

        INSERT_ROW_NEXT -> ',' GROUPED_OBJECT_LIST INSERT_ROW_NEXT
        INSERT_ROW_NEXT -> ''

        TARGET_COLUMNS -> '*'
        TARGET_COLUMNS -> OBJECT_LIST

        GROUPED_OBJECT_LIST -> '(' OBJECT_LIST ')'

        OBJECT_LIST -> object OBJECT_LIST_NEXT

        OBJECT_LIST_NEXT -> ',' object OBJECT_LIST_NEXT
        OBJECT_LIST_NEXT -> ''

        WHERE_CLAUSE -> where CONDITION
        WHERE_CLAUSE -> ''

        CONDITION -> '(' CONDITION ')'
        CONDITION -> object operator CONDITION_TARGET CONDITION_NEXT

        CONDITION_NEXT -> and CONDITION
        CONDITION_NEXT -> or CONDITION
        CONDITION_NEXT -> ''

        CONDITION_TARGET -> object
        CONDITION_TARGET -> VALUE

        VALUE -> string_literal
        VALUE -> integer
    */

    internal class Parser
    {
        private readonly Stack<Token> _tokens;
        private readonly Token _endOfSequenceToken = new Token(TokenType.EndOfSequence, null);

        private Token _current;
        private Token Current => _current;
        private Token Lookahead =>
            _tokens.Any() ? _tokens.Peek() : _endOfSequenceToken;

        private void ExpectToken(TokenType type)
        {
            if (Current.Type != type)
            {
                throw new ParserException($"Expected {type} but found {Current.Type}");
            }
        }

        private void DiscardToken() =>
            _current = _tokens.Any() ? _tokens.Pop() : _endOfSequenceToken;

        private void DiscardToken(TokenType type)
        {
            ExpectToken(type);
            DiscardToken();
        }

        public Parser(IEnumerable<Token> tokens)
        {
            _tokens = new Stack<Token>();
            foreach (var token in tokens.Reverse())
            {
                if (token.Type == TokenType.Whitespace) continue;
                _tokens.Push(token);
            }

            // Initialize _current by popping the first token on the stack
            DiscardToken();

        }

        public QueryModel Parse()
        {
            if (Current.Type == TokenType.SelectKeyword)
            {
                return ParseSelect();
            }
            else if (Current.Type == TokenType.InsertKeyword)
            {
                return ParseInsert();
            }
            else if (Current.Type == TokenType.DeleteKeyword)
            {
                return ParseDelete();
            }
            else
            {
                throw new ParserException("Expected SELECT, INSERT, or DELETE");
            }
        }

        private QueryModel ParseSelect()
        {
            var queryModel = new SelectQueryModel();

            DiscardToken(TokenType.SelectKeyword);

            var targetColumns = ParseObjectList();
            queryModel.TargetColumns = targetColumns;

            DiscardToken(TokenType.FromKeyword);

            ExpectToken(TokenType.Object);
            queryModel.TargetTable = Current.Value;
            DiscardToken();

            ExpectToken(TokenType.EndOfSequence);

            return queryModel;
        }

        private QueryModel ParseInsert()
        {
            return new InsertQueryModel();
        }

        private QueryModel ParseDelete()
        {
            return null;
        }

        private IList<string> ParseObjectList()
        {
            var objectList = new List<string>();

            ExpectToken(TokenType.Object);
            objectList.Add(Current.Value);
            DiscardToken();

            ParseObjectListNext(objectList);

            return objectList;
        }

        private void ParseObjectListNext(IList<string> objectList)
        {
            if (Current.Type != TokenType.Comma) return;
            DiscardToken();

            ExpectToken(TokenType.Object);
            objectList.Add(Current.Value);
            DiscardToken();

            ParseObjectListNext(objectList);
        }
    }
}