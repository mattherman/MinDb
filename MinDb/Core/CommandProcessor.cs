using System;
using System.Linq;
using MinDb.Compiler;
using MinDb.Compiler.Models;

namespace MinDb.Core
{
    internal class CommandProcessor
    {
        private readonly VirtualMachine _vm;

        public CommandProcessor(string databaseFilename)
        {
            _vm = new VirtualMachine(databaseFilename);
        }

        public QueryResult Process(string query)
        {
            var lexer = new Lexer(query);
            var tokens = lexer.Tokenize();

            Console.WriteLine($"[DEBUG] Tokens = [{string.Join(", ", tokens)}]");

            var parser = new Parser(tokens);
            var queryModel = parser.Parse();

            Console.WriteLine($"[DEBUG] {queryModel}");

            return _vm.Execute(queryModel);
        }
    }
}