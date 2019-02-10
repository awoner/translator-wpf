using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Translator_desktop.LexicalAnalyzer.Tables;

namespace Translator_desktop.LexicalAnalyzer.Tables
{
    static class TokenTable
    {
        private static string[] tokens = { "double", "float", "int", /*"bool" ,*/ "for", "if",
            "cin", "cout", "{", "}", ";", ",", "=", "<<", ">>", "<", ">", "<=",
            ">=", "==", "!=", "+", "-", "*", "/", "(", ")", "true", "false", "IDN", "CON" };

        private static IList<Token> tokenTable = new List<Token>();

        public static void InitTable()
        {
            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] == "IDN")
                    tokenTable.Add(new Token { Code = 100, Name = tokens[i] });
                else if(tokens[i] == "CON")
                    tokenTable.Add(new Token { Code = 101, Name = tokens[i] });
                else
                    tokenTable.Add(new Token { Code = i + 1, Name = tokens[i] });
            }
        }

        public static void Show()
        {
            Console.WriteLine("    Table of Tokens");
            Console.WriteLine("-------------------------");
            Console.WriteLine("|   Code   |    Name    |");
            foreach (Token token in tokenTable)
            {
                Console.WriteLine("-------------------------");
                Console.WriteLine("|{0, 6}    |{1, 9}   |", token.Code, token.Name);
            }
            Console.WriteLine("-------------------------");
        }

        public static int GetCode(string token)
        {
            foreach (Token t in tokenTable)
            {
                if (t.Name == token)
                {
                    return t.Code;
                }
            }

            return 0;
        }

        public static bool Contains(string token)
        {
            foreach (Token t in tokenTable)
            {
                if (t.Name == token)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
