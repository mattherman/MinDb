using System;

internal static class String
{
    public static bool Matches(this string value, int index, Func<char, bool> predicate)
    {
        if (index >= value.Length) return false;
        return predicate(value[index]);
    } 
}