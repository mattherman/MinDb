using System;

namespace MinDb.Compiler
{
    internal class ParserException : Exception
    {
        public ParserException(string message) : base(message) { }
    }
}