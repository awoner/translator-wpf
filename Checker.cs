using System;
using System.Globalization;
using System.Linq;
using Translator_desktop.LexicalAnalyzer.Tables;

namespace Translator_desktop
{
    static class Checker
    {
        public static string Type { get; set; }

        public static bool IsWhiteSeparator(char ch)
        {
            return (char.IsWhiteSpace(ch) || char.IsControl(ch));
        }

        public static bool IsConstant(char ch)
        {
            return (char.IsNumber(ch) || ch == '.' || ch == 'E' || ch == 'e');
        }

        public static bool IsExponent(char ch)
        {
            return (ch == 'e' || ch == 'E');
        }

        public static bool IsLetter(char ch)
        {
            return (char.IsLetter(ch));
        }

        public static bool IsSingleCharacterSeparator(char ch)
        {
            return (ch == '[' || ch == ']' || ch == '{' || ch == '}' ||
                        ch == '(' || ch == ')' || ch == ',' || ch == ';' ||
                        ch == '+' || ch == '-' || ch == '*' || ch == '/');
        }

        public static bool IsPossibleDoubleSeparator(char ch)
        {
            return (ch == '=' || ch == '<' || ch == '>' || ch == '!');
        }

        public static bool IsDoubleSeparator(string str)
        {
            return (str == "==" || str == ">>" || str == "<<" || str == ">=" || str == "<=" || str == "!=");
        }

        public static bool IsType(string str)
        {
            return (str == "int" || str == "double" || str == "float" /*|| str == "bool"*/);
        }

        public static bool IsSign(char ch)
        {
            return (ch == '+' || ch == '-');
        }

        public static string GetType(string str)
        {
            if (Checker.IsDigit(str) && str.Contains('.') && (str.Contains('e') || str.Contains('E')))
            {
                return "double";
            }
            else if (Checker.IsDigit(str) && str.Contains('.'))
            {
                return "float";
            }
            else if (Checker.IsDigit(str))
            {
                return "int";
            }
            //else if (str == "true" || str == "false")
            //{
            //    return "bool";
            //}
            else
            {
                throw new Exception("Can't called method GetType() to no digit token!");
            }
        }

        public static bool IsDigit(string str)
        {
            decimal result;
            return decimal.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
        }

        public static bool IsAbleToCast(string token)
        {
            //output tokens is exist and last token in table is "="
            return OutputTokenTable.Table.Count != 0 && OutputTokenTable.Table.Last().Name == "="

                //penultimate token is identifier
                && IdnTable.Contains(OutputTokenTable.Table[OutputTokenTable.Table.Count - 2].Name)

                //current token is identifier
                && IdnTable.Contains(token)

                //types of tokens on both sides of "=" are equal
                && (IdnTable.GetType(token) == IdnTable.GetType(OutputTokenTable.Table[OutputTokenTable.Table.Count - 2].Name));
        }
    }
}