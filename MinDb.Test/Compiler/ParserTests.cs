using System;
using Xunit;
using MinDb.Compiler;
using System.Linq;
using MinDb.Compiler.Models;
using ValueType = MinDb.Compiler.Models.ValueType;

namespace MinDb.Tests
{
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
            // select FirstName, LastName from Users
            var model = Parse<SelectQueryModel>(
                new Token(TokenType.SelectKeyword, null),
                new Token(TokenType.Object, "FirstName"),
                new Token(TokenType.Comma, null),
                new Token(TokenType.Object, "LastName"),
                new Token(TokenType.FromKeyword, null),
                new Token(TokenType.Object, "Users")
            );

            var columns = model.TargetColumns.ToList();
            Assert.Equal("FirstName", columns[0].Name);
            Assert.Equal("LastName", columns[1].Name);

            Assert.Equal("Users", model.TargetTable.Name);
        }

        [Fact]
        public void Parse_SelectWithSingleCondition()
        {
            // select FirstName from Users where LastName = 'Herman'
            var model = Parse<SelectQueryModel>(
                new Token(TokenType.SelectKeyword, null),
                new Token(TokenType.Object, "FirstName"),
                new Token(TokenType.FromKeyword, null),
                new Token(TokenType.Object, "Users"),
                new Token(TokenType.WhereKeyword, null),
                new Token(TokenType.Object, "LastName"),
                new Token(TokenType.Equal, "="),
                new Token(TokenType.StringLiteral, "Smith")
            );

            AssertCondition("LastName", OperatorType.Equal, ValueType.String, "Smith", model.Condition);
        }

        [Fact]
        public void Parse_SelectWithMultiCondition()
        {
            // select FirstName from Users where LastName = 'Smith' or (LastName = 'Doe' and (Age >= 20 and Age < 40))
            var model = Parse<SelectQueryModel>(
                new Token(TokenType.SelectKeyword, null),
                new Token(TokenType.Object, "FirstName"),
                new Token(TokenType.FromKeyword, null),
                new Token(TokenType.Object, "Users"),
                new Token(TokenType.WhereKeyword, null),
                new Token(TokenType.Object, "LastName"),
                new Token(TokenType.Equal, "="),
                new Token(TokenType.StringLiteral, "Smith"),
                new Token(TokenType.OrKeyword, null),
                new Token(TokenType.OpenParenthesis, null),
                new Token(TokenType.Object, "LastName"),
                new Token(TokenType.Equal, "="),
                new Token(TokenType.StringLiteral, "Doe"),
                new Token(TokenType.AndKeyword, null),
                new Token(TokenType.OpenParenthesis, null),
                new Token(TokenType.Object, "Age"),
                new Token(TokenType.GreaterThanOrEqual, ">="),
                new Token(TokenType.Integer, "20"),
                new Token(TokenType.AndKeyword, null),
                new Token(TokenType.Object, "Age"),
                new Token(TokenType.LessThan, null),
                new Token(TokenType.Integer, "40"),
                new Token(TokenType.CloseParenthesis, null),
                new Token(TokenType.CloseParenthesis, null)
            );

            var rootOperation = model.Condition.Value as OperatorModel;
            Assert.NotNull(rootOperation);
            Assert.Equal(OperatorType.Or, rootOperation.Type);

            AssertCondition("LastName", OperatorType.Equal, ValueType.String, "Smith", model.Condition.Left);
            AssertCondition("LastName", OperatorType.Equal, ValueType.String, "Doe", model.Condition.Right.Left);
            AssertCondition("Age", OperatorType.GreaterThanOrEqual, ValueType.Integer, "20", model.Condition.Right.Right.Left);
            AssertCondition("Age", OperatorType.LessThan, ValueType.Integer, "40", model.Condition.Right.Right.Right);
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
        public void Parse_Select_EndOfSequenceMustFollowWhereClause()
        {
            Assert.Throws<ParserException>(() =>
                Parse<SelectQueryModel>(
                    new Token(TokenType.SelectKeyword, null),
                    new Token(TokenType.Object, "FirstName"),
                    new Token(TokenType.FromKeyword, null),
                    new Token(TokenType.Object, "Users"),
                    new Token(TokenType.WhereKeyword, null),
                    new Token(TokenType.Object, "LastName"),
                    new Token(TokenType.Equal, "="),
                    new Token(TokenType.StringLiteral, "Smith"),
                    new Token(TokenType.FromKeyword, null)
                )
            );
        }

        [Fact]
        public void Parse_Insert()
        {
            // insert into Users values (1, 'John'), (2, 'Jane')
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
                new Token(TokenType.StringLiteral, "Jane"),
                new Token(TokenType.CloseParenthesis, null)
            );

            Assert.Equal("Users", model.TargetTable.Name);
        }

        [Fact]
        public void Parse_Insert_IntoMustFollowInsert()
        {
            Assert.Throws<ParserException>(() =>
                Parse<InsertQueryModel>(
                    new Token(TokenType.InsertKeyword, null),
                    new Token(TokenType.Object, "Users")
                )
            );
        }

        [Fact]
        public void Parse_Insert_TableMustFollowInto()
        {
            Assert.Throws<ParserException>(() =>
                Parse<InsertQueryModel>(
                    new Token(TokenType.InsertKeyword, null),
                    new Token(TokenType.IntoKeyword, null),
                    new Token(TokenType.ValuesKeyword, null)
                )
            );
        }

        [Fact]
        public void Parse_Insert_ValuesMustFollowTable()
        {
            Assert.Throws<ParserException>(() =>
                Parse<InsertQueryModel>(
                    new Token(TokenType.InsertKeyword, null),
                    new Token(TokenType.IntoKeyword, null),
                    new Token(TokenType.Object, "Users"),
                    new Token(TokenType.OpenParenthesis, null)
                )
            );
        }

        [Fact]
        public void Parse_Insert_RowMustFollowValues()
        {
            Assert.Throws<ParserException>(() =>
                Parse<InsertQueryModel>(
                    new Token(TokenType.InsertKeyword, null),
                    new Token(TokenType.IntoKeyword, null),
                    new Token(TokenType.Object, "Users"),
                    new Token(TokenType.ValuesKeyword, null),
                    new Token(TokenType.WhereKeyword, null)
                )
            );
        }

        [Fact]
        public void Parse_Insert_EndOfSequenceMustFollowGroupedValues()
        {
            Assert.Throws<ParserException>(() =>
                Parse<InsertQueryModel>(
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
                    new Token(TokenType.StringLiteral, "Jane"),
                    new Token(TokenType.CloseParenthesis, null),
                    new Token(TokenType.ValuesKeyword, null)
                )
            );
        }

        [Fact]
        public void Parse_Delete()
        {
            // delete from Users
            var model = Parse<DeleteQueryModel>(
                new Token(TokenType.DeleteKeyword, null),
                new Token(TokenType.FromKeyword, null),
                new Token(TokenType.Object, "Users")
            );

            Assert.Equal("Users", model.TargetTable.Name);
        }

        [Fact]
        public void Parse_Delete_FromMustFollowDelete()
        {
            Assert.Throws<ParserException>(() =>
                Parse<InsertQueryModel>(
                    new Token(TokenType.DeleteKeyword, null),
                    new Token(TokenType.Object, "Users")
                )
            );
        }

        [Fact]
        public void Parse_Delete_TableMustFollowFrom()
        {
            Assert.Throws<ParserException>(() =>
                Parse<InsertQueryModel>(
                    new Token(TokenType.DeleteKeyword, null),
                    new Token(TokenType.FromKeyword, null),
                    new Token(TokenType.WhereKeyword, null)
                )
            );
        }

        [Fact]
        public void Parse_Delete_EndOfSequenceMustFollowTable()
        {
            Assert.Throws<ParserException>(() =>
                Parse<InsertQueryModel>(
                    new Token(TokenType.DeleteKeyword, null),
                    new Token(TokenType.FromKeyword, null),
                    new Token(TokenType.Object, "Users"),
                    new Token(TokenType.WhereKeyword, null)
                )
            );
        }

        private void AssertCondition(string expectedObjectName, OperatorType expectedOperator, ValueType expectedValueType, string expectedValue, BinaryTree<IConditionNode> condition)
        {
            var left = condition.Left.Value as ObjectModel;
            Assert.NotNull(left);
            Assert.Equal(expectedObjectName, left.Name);

            var op = condition.Value as OperatorModel;
            Assert.NotNull(op);
            Assert.Equal(expectedOperator, op.Type);

            var right = condition.Right.Value as ValueModel;
            Assert.NotNull(right);
            Assert.Equal(expectedValueType, right.Type);
            Assert.Equal(expectedValue, right.Value);
        }
    }
}