using System;
using MinDb.Core;

namespace MinDb.Repl
{
    class Program
    {
        static void Main(string[] args)
        {
            var input = "";

            var db = new Database("min.db");

            while (true)
            {
                Console.Write("db > ");
                //input = Console.ReadLine();
                input = "Select FirstName, LastName, Age";
                if (string.Compare(input, ":q", true) == 0) break;

                var result = db.Execute(input);

                Console.WriteLine(result);
            }
        }
    }
}
