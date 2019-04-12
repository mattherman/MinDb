using System;

namespace MinDb.Compiler
{
    internal class LexerException : Exception
    {
        public LexerException(string message) : base(message) { }
    }
}