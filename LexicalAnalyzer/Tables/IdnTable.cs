using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Translator_desktop.LexicalAnalyzer.Tables;

namespace Translator_desktop.LexicalAnalyzer.Tables
{
    static class IdnTable
    {
        private static int code = 1;
        private static IList<Token> idnTable;

        public static void Show()
        {
            Console.WriteLine("\tTable of Identifiers");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("|   №   |    Name    |    Type    |");
            Console.WriteLine("-----------------------------------");
            foreach (Token token in idnTable)
            {
                Console.WriteLine("|{0, 4}   |{1, 8}    |{2, 8}    |", token.Code, token.Name, token.Type);
                Console.WriteLine("-----------------------------------");
            }
        }

        public static IList<Token> GetTable()
        {
            return idnTable;
        }

        public static void InitTable()
        {
            idnTable = new List<Token>();
        }

        public static string GetType(string token)
        {
            foreach (Token idn in idnTable)
            {
                if (idn.Name == token)
                {
                    return idn.Type;
                }
            }

            return "undefined";
        }

        public static int GetCode(string token)
        {
            foreach (Token idn in idnTable)
            {
                if (idn.Name == token)
                {
                    return idn.Code;
                }
            }

            return 0;
        }

        public static bool Contains(string identifier)
        {
            foreach (Token idn in idnTable)
            {
                if (idn.Name == identifier)
                    return true;
            }
            return false;
        }

        public static void Add(string token)
        {
            idnTable.Add(new Token { Code = code, Name = token, Type = Checker.Type });
            code++;
        }
    }
}
