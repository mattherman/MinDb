using System;

internal class ParserException : Exception
{
    public ParserException(string message) : base(message) { }
}