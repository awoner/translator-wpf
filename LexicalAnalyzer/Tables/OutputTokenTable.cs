using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Translator_desktop.LexicalAnalyzer.Tables;

namespace Translator_desktop.LexicalAnalyzer.Tables
{
    static class OutputTokenTable
    {
        private static int number = 1;
        private static IList<Token> outputTokenTable;

        public static void Show()
        {
            Console.WriteLine("\t\t\t    Source Table of Tokens");
            Console.WriteLine("----------------------------------------------------------------------------");
            Console.WriteLine("|   №   |   Row   |    Name    |   TokenCode   |   IdnCode   |   ConCode   |");
            Console.WriteLine("----------------------------------------------------------------------------");

            foreach (Token token in outputTokenTable)
            {
                Console.WriteLine("|{0, 4}   |{1, 6}   |{2, 8}    |{3, 9}      |{4,7}      |{5,7}      |", 
                              token.Code, token.Row, token.Name, token.TokenCode, token.IdnCode, token.ConCode);
                Console.WriteLine("----------------------------------------------------------------------------");
            }
        }

        public static IList<Token> GetTable()
        {
            return outputTokenTable;
        }

        public static void InitTable()
        {
            outputTokenTable = new List<Token>();
            number = 1;
        }

        public static void Add(int numRow, string token)
        {
            if (TokenTable.Contains(token) || ConTable.Contains(token) || IdnTable.Contains(token))
            {
                outputTokenTable.Add(new Token
                {
                    Code = number,
                    Row = numRow,
                    Type = Checker.Type,
                    Name = token,
                    IdnCode = IdnTable.GetCode(token),
                    ConCode = ConTable.GetCode(token),
                    TokenCode = IdnTable.GetCode(token) != 0 ? 
                            TokenTable.GetCode("IDN") : ConTable.GetCode(token) != 0 ?
                                    TokenTable.GetCode("CON") : TokenTable.GetCode(token)
                });
                number++;
            }
            else
            {
                throw new Exception("Error on " + numRow + " line!\tUndeclarated identifier \"" + token + "\".");
            }
        }
    }
}
