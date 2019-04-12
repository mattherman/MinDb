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

        INSERT -> insert into object values INSERT_ROW

        DELETE -> delete from object WHERE_CLAUSE

        INSERT_ROW -> GROUPED_VALUE_LIST INSERT_ROW_NEXT

        INSERT_ROW_NEXT -> ',' GROUPED_VALUE_LIST INSERT_ROW_NEXT
        INSERT_ROW_NEXT -> ''

        TARGET_COLUMNS -> '*'
        TARGET_COLUMNS -> OBJECT_LIST

        GROUPED_VALUE_LIST -> '(' VALUE_LIST ')'

        VALUE_LIST -> VALUE VALUE_LIST_NEXT

        VALUE_LIST_NEXT -> ',' VALUE VALUE_LIST_NEXT
        VALUE_LIST_NEXT -> ''

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
            foreach (var token in tokens.Reverse().Where(t => t.Type != TokenType.Whitespace))
            {
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

            queryModel.TargetColumns = ParseObjectList();

            DiscardToken(TokenType.FromKeyword);

            queryModel.TargetTable = ParseObject();

            ExpectToken(TokenType.EndOfSequence);

            return queryModel;
        }

        private QueryModel ParseInsert()
        {
            var queryModel = new InsertQueryModel();

            DiscardToken(TokenType.InsertKeyword);
            DiscardToken(TokenType.IntoKeyword);

            queryModel.TargetTable = ParseObject();

            DiscardToken(TokenType.ValuesKeyword);

            queryModel.Rows = ParseInsertRow();

            ExpectToken(TokenType.EndOfSequence);

            return queryModel;
        }

        private QueryModel ParseDelete()
        {
            var queryModel = new DeleteQueryModel();

            DiscardToken(TokenType.DeleteKeyword);
            DiscardToken(TokenType.FromKeyword);

            queryModel.TargetTable = ParseObject();

            ExpectToken(TokenType.EndOfSequence);

            return queryModel;
        }

        private IList<ValueModelGroup> ParseInsertRow()
        {
            var rows = new List<ValueModelGroup>();

            rows.Add(ParseGroupedValueList());

            ParseInsertRowNext(rows);

            return rows;
        }

        private void ParseInsertRowNext(IList<ValueModelGroup> rows)
        {
            if (Current.Type != TokenType.Comma) return;
            DiscardToken();

            rows.Add(ParseGroupedValueList());

            ParseInsertRowNext(rows);
        }

        private ValueModelGroup ParseGroupedValueList()
        {
            var group = new ValueModelGroup();
            DiscardToken(TokenType.OpenParenthesis);
            group.Values = ParseValueList();
            DiscardToken(TokenType.CloseParenthesis);

            return group;
        }

        private IList<ValueModel> ParseValueList()
        {
            var values = new List<ValueModel>();

            values.Add(ParseValue());

            ParseValueListNext(values);

            return values;
        }

        private void ParseValueListNext(IList<ValueModel> values)
        {
            if (Current.Type != TokenType.Comma) return;
            DiscardToken();

            values.Add(ParseValue());

            ParseValueListNext(values);
        }

        private ValueModel ParseValue()
        {
            if (Current.Type == TokenType.StringLiteral)
            {
                return ParseStringLiteral();
            }
            else
            {
                return ParseInteger();
            }
        }

        private ValueModel ParseStringLiteral()
        {
            ExpectToken(TokenType.StringLiteral);
            var valueModel = new ValueModel(ValueType.String, Current.Value);
            DiscardToken();

            return valueModel;
        }

        private ValueModel ParseInteger()
        {
            ExpectToken(TokenType.Integer);
            var valueModel = new ValueModel(ValueType.Integer, Current.Value);
            DiscardToken();

            return valueModel;
        }

        private IList<ObjectModel> ParseObjectList()
        {
            var objects = new List<ObjectModel>();

            objects.Add(ParseObject());

            ParseObjectListNext(objects);

            return objects;
        }

        private void ParseObjectListNext(IList<ObjectModel> objects)
        {
            if (Current.Type != TokenType.Comma) return;
            DiscardToken();

            objects.Add(ParseObject());

            ParseObjectListNext(objects);
        }

        private ObjectModel ParseObject()
        {
            ExpectToken(TokenType.Object);
            var objectModel = new ObjectModel(Current.Value);
            DiscardToken();

            return objectModel;
        }
    }
}