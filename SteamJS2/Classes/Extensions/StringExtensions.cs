using System;

namespace SteamJS2.Extensions
{
    internal static class StringExtensions
    {
        public static string ToCamelCase(this string str)
        {
            if (str == null)
                throw new NullReferenceException();

            if (str == string.Empty)
                return str;

            if (str.Length == 1)
                return str.ToLower();

            if (str.ToUpper() == str) // Don't change if it's all caps
                return str;

            return str[0].ToString().ToLower() + str.Substring(1);
        }
    }
}