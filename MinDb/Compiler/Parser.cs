using System.Collections.Generic;
using System.Linq;
using MinDb.Compiler.Models;
using System;
using ValueType = MinDb.Compiler.Models.ValueType;

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

        CONDITION -> '(' CONDITION ')' CONDITION_NEXT
        CONDITION -> object operator VALUE CONDITION_NEXT

        CONDITION_NEXT -> and CONDITION
        CONDITION_NEXT -> or CONDITION
        CONDITION_NEXT -> ''

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
                UnexpectedToken(type);
            }
        }

        private void DiscardToken() =>
            _current = _tokens.Any() ? _tokens.Pop() : _endOfSequenceToken;

        private void DiscardToken(TokenType type)
        {
            ExpectToken(type);
            DiscardToken();
        }

        private void UnexpectedToken(params TokenType[] expectedTokens)
        {
            var message = expectedTokens.Length == 1 ?
                $"Expected {expectedTokens[0]} but found {Current.Type}" :
                $"Expected one of {string.Join(",", expectedTokens)} but found {Current.Type}";
            throw new ParserException(message);
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
                UnexpectedToken(TokenType.SelectKeyword, TokenType.InsertKeyword, TokenType.DeleteKeyword);
                return null;
            }
        }

        private QueryModel ParseSelect()
        {
            var queryModel = new SelectQueryModel();

            DiscardToken(TokenType.SelectKeyword);

            queryModel.TargetColumns = ParseObjectList();

            DiscardToken(TokenType.FromKeyword);

            queryModel.TargetTable = ParseObject();

            queryModel.Condition = ParseWhereClause();

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

            queryModel.Condition = ParseWhereClause();

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

        private ConditionModel ParseWhereClause()
        {
            if (Current.Type != TokenType.WhereKeyword) return null;
            DiscardToken();

            return ParseCondition();
        }

        private ConditionModel ParseCondition()
        {
            ConditionModel currentCondition;
            if (Current.Type == TokenType.OpenParenthesis)
            {
                DiscardToken();
                currentCondition = ParseCondition();
                DiscardToken(TokenType.CloseParenthesis);
            }
            else
            {
                var obj = ParseObject();
                var op = ParseOperator();
                var target = ParseValue();
                currentCondition = new ConditionModel 
                {
                    Left = new ConditionModel { Value = obj },
                    Value = op,
                    Right = new ConditionModel { Value = target }
                };
            }

            return ParseConditionNext(currentCondition);
        }

        private ConditionModel ParseConditionNext(ConditionModel previousCondition)
        {
            if (Current.Type == TokenType.AndKeyword)
            {
                DiscardToken();
                var nextCondition = ParseCondition();
                var op = new OperatorModel(OperatorType.And);
                return new ConditionModel
                {
                    Left = previousCondition,
                    Value = op,
                    Right = nextCondition
                };
            }
            else if (Current.Type == TokenType.OrKeyword)
            {
                DiscardToken();
                var nextCondition = ParseCondition();
                var op = new OperatorModel(OperatorType.Or);
                return new ConditionModel
                {
                    Left = previousCondition,
                    Value = op,
                    Right = nextCondition
                };
            }
            else
            {
                return previousCondition;
            }
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

        private OperatorModel ParseOperator()
        {
            var type = OperatorType.Unknown;
            switch (Current.Type)
            {
                case TokenType.Equal:
                    type = OperatorType.Equal;
                    break;
                case TokenType.NotEqual:
                    type = OperatorType.NotEqual;
                    break;
                case TokenType.LessThan:
                    type = OperatorType.LessThan;
                    break;
                case TokenType.LessThanOrEqual:
                    type = OperatorType.LessThanOrEqual;
                    break;
                case TokenType.GreaterThan:
                    type = OperatorType.GreaterThan;
                    break;
                case TokenType.GreaterThanOrEqual:
                    type = OperatorType.GreaterThanOrEqual;
                    break;
                default:
                    UnexpectedToken(
                        TokenType.Equal,
                        TokenType.NotEqual,
                        TokenType.LessThan, 
                        TokenType.LessThanOrEqual,
                        TokenType.GreaterThan, 
                        TokenType.GreaterThanOrEqual
                    );
                    break;
            }

            var operatorModel = new OperatorModel(type);
            DiscardToken();

            return operatorModel;
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