using System;
using System.Text;

namespace System
{
    public static class StringExtensions
    {
        public static bool StartsWith(this string s, string value)
        {
            return s.IndexOf(value) == 0;
        }

        public static bool Contains(this string s, string value)
        {
            return s.IndexOf(value) > 0;
        }

        public static bool EndsWith(this string s, string value)
        {
            return s.IndexOf(value) == s.Length - value.Length;
        }
    }
}
