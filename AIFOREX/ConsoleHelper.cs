using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace AIFOREX
{
    public static class ConsoleHelper
    {
        public static void WriteHebrew(string text)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine(FixHebrewDirection(text));
        }

        public static string FixHebrewDirection(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            if (IsHebrewOnly(input))
                return ReverseText(input);

            return ReverseHebrewWordsIndividually(input);
        }

        private static string ReverseText(string text)
        {
            var chars = text.ToCharArray();
            Array.Reverse(chars);
            return new string(chars);
        }

        private static bool IsHebrewOnly(string text)
        {
            return text.All(c => char.IsWhiteSpace(c) || (c >= '\u0590' && c <= '\u05FF'));
        }

        private static string ReverseHebrewWordsIndividually(string input)
        {
            var words = input.Split(' ');
            for (int i = 0; i < words.Length; i++)
            {
                if (Regex.IsMatch(words[i], @"\p{IsHebrew}"))
                    words[i] = ReverseText(words[i]);
            }
            return string.Join(" ", words);
        }
    }
}
