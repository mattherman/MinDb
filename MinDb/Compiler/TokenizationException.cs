using System;

namespace MinDb.Compiler
{
    internal class TokenizationException : Exception
    {
        public TokenizationException(string message) : base(message) { }
    }
}