using System;
using System.Linq;
using System.Text;
using MinDb.Compiler.Models;
using MinDb.Core;

namespace MinDb.Storage
{
    internal class Store
    {
        private readonly Table _users;
        private readonly string _databaseFilename;

        public Store(string databaseFilename)
        {
            _databaseFilename = databaseFilename;
            _users = new Table();
        }

        public QueryResult Select(SelectQueryModel query)
        {
            return null;
        }

        public QueryResult Insert(InsertQueryModel query)
        {
            if (string.Compare(query.TargetTable.Name, "users") != 0)
            {
                throw new DataAccessException("Table does not exist");
            }

            foreach (var row in query.Rows)
            {
                var values = row.Values.ToList();
                _users.Insert(int.Parse(values[0].Value), values[1].Value, values[2].Value);
            }

            return new QueryResult();
        }

        public QueryResult Delete(DeleteQueryModel query)
        {
            return null;
        }
    }

    internal class Table
    {
        private readonly byte[,] _pages;
        private int _currentPage;
        private int _currentIndex;

        public Table()
        {
            _pages = new byte[TableInfo.TableMaxPages, TableInfo.PageSize];
        }

        public void Insert(int id, string username, string email)
        {
            if (_currentIndex + TableInfo.RowSize > TableInfo.PageSize)
            {
                if (++_currentPage >= TableInfo.TableMaxPages)
                {
                    throw new Exception("Failed to insert record, no remaining space in table");
                }
            }

            CopyToPage(TableInfo.IdOffset, id);
            CopyToPage(TableInfo.UsernameOffset, username);
            CopyToPage(TableInfo.EmailOffset, email);

            _currentIndex += TableInfo.RowSize;
        }

        private void CopyToPage(int offset, int value)
        {
            _pages[_currentPage, _currentIndex + offset] = (byte)(value >> 24);
            _pages[_currentPage, _currentIndex + offset + 1] = (byte)(value >> 16);
            _pages[_currentPage, _currentIndex + offset + 2] = (byte)(value >> 8 );
            _pages[_currentPage, _currentIndex + offset + 3] = (byte)value;
        }

        private void CopyToPage(int offset, string value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);
            for (var i=0; i<valueBytes.Length; i++)
            {
                _pages[_currentPage, _currentIndex + offset + i] = valueBytes[i];
            }
        }
    }

    internal static class TableInfo
    {
        /*
            CREATE TABLE Users (
                Id          int
                Username   varchar(32)
                Email      varchar(255)
            )
        */

        public const int IdSize = 4;
        public const int UsernameSize = 32;
        public const int EmailSize = 255;

        public const int IdOffset = 0;
        public const int UsernameOffset = IdSize;
        public const int EmailOffset = IdSize + UsernameSize;

        public const int RowSize = IdSize + UsernameSize + EmailSize;
        public const int PageSize = 4096;

        public const int RowsPerPage = PageSize / RowSize;
        public const int TableMaxPages = 100;
        public const int TableMaxRows = TableMaxPages * RowsPerPage;
    }
}