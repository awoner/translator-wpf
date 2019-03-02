using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Translator_desktop.LexicalAnalyzer.Tables;

namespace Translator_desktop.LexicalAnalyzer.Tables
{
    /// <summary>
    /// The class interprets a table of output tokens
    /// </summary>
    static class OutputTokenTable
    {
        private static int number = 1;
        private static IList<Token> outputTokenTable;
        public static IList<Token> Table { get => outputTokenTable; }

        /// <summary>
        /// Return a string representation of the table
        /// </summary>
        public static string GetStringTable()
        {
            StringBuilder str = new StringBuilder("\t\t\t    Source Table of Tokens");
            string separator = new string('-', 76) + '\n';

            str.Append(separator);
            str.Append("|   №   |   Row   |    Name    |   TokenCode   |   IdnCode   |   ConCode   |\n");
            str.Append(separator);

            foreach (Token token in outputTokenTable)
            {
                str.AppendFormat("|{0, 4}   |{1, 6}   |{2, 8}    |{3, 9}      |{4,7}      |{5,7}      |\n",
                              token.Code, token.Row, token.Name, token.TokenCode, token.IdnCode, token.ConCode);
                str.Append(separator);
            }

            return str.ToString();
        }

        /// <summary>
        /// Initialization of tables of output tokens
        /// </summary>
        public static void InitTable()
        {
            number = 1;
            outputTokenTable = new List<Token>();
        }

        /// <summary>
        /// Add a token in the table
        /// </summary>
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
