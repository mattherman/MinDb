using System;
using MinDb.Core;

namespace MinDb.Repl
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new Database("min.db");

            if (args.Length > 0)
            {
                ExecuteQuery(db, args[0]);
            }
            else
            {
                while (true)
                {
                    var input = "";

                    Console.Write("db > ");
                    input = Console.ReadLine();

                    if (string.Compare(input, ":q", true) == 0) break;

                    ExecuteQuery(db, input);
                }
            }
        }

        static void ExecuteQuery(Database db, string input)
        {
            var result = db.Execute(input);
            Console.WriteLine(result);
        }
    }
}
