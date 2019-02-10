using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Translator_desktop.LexicalAnalyzer.Tables;

namespace Translator_desktop.LexicalAnalyzer.Tables
{
    static class ConTable
    {
        private static int code = 1;
        private static IList<Token> conTable;

        public static void Show()
        {
            Console.WriteLine("\tTable of Constants");
            Console.WriteLine("-----------------------------------");
            Console.WriteLine("|   №   |    Name    |    Type    |");
            Console.WriteLine("-----------------------------------");
            foreach (Token token in conTable)
            {
                Console.WriteLine("|{0, 4}   |{1, 9}   |{2, 8}    |", token.Code, token.Name, token.Type);
                Console.WriteLine("-----------------------------------");
            }
        }

        public static IList<Token> GetTable()
        {
            return conTable;
        }

        public static void InitTable()
        {
            conTable = new List<Token>();
        }

        public static int GetCode(string token)
        {
            foreach (Token con in conTable)
            {
                if (con.Name == token)
                {
                    return con.Code;
                }
            }

            return 0;
        }

        public static bool Contains(string constant)
        {
            foreach (Token con in conTable)
            {
                if (con.Name == constant)
                    return true;
            }
            return false;
        }

        public static void Add(string token, string type = null)
        {
            if (token.Contains('.') && (token.Contains('e') || token.Contains('E')) && type == null)
            {
                conTable.Add(new Token { Code = code, Name = token, Type = "double" });
                code++;
            }
            else if (token.Contains('.') && type == null)
            {
                conTable.Add(new Token { Code = code, Name = token, Type = "float" });
                code++;
            }
            else if (type == null)
            {
                conTable.Add(new Token { Code = code, Name = token, Type = "int" });
                code++;
            }
            else
            {
                conTable.Add(new Token { Code = code, Name = token, Type = type });
                code++;
            }
        }
    }
}
