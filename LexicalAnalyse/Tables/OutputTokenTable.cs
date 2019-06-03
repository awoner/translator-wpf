using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Translator_desktop.LexicalAnalyse.Tables
{
    /// <summary>
    /// The class interprets a table of output tokens
    /// </summary>
    static class OutputTokenTable
    {
        private static int number = 1;
        public static List<Token> Table { get; private set; }

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

            foreach (Token token in Table)
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
            Table = new List<Token>();
        }

        /// <summary>
        /// Add a token in the table
        /// </summary>
        public static void Add(int numRow, string token)
        {
            string GetTokenType()
            {
                if (IdnTable.Contains(token))
                {
                    return "IDN";
                }
                else if (ConTable.Contains(token))
                {
                    return "CON";
                }
                else if (token.Equals("-") && !(Table.Last().TokenType.Equals("IDN") || Table.Last().TokenType.Equals("CON") || Table.Last().Name.Equals(")")))
                {
                    return "@";
                }

                return string.Empty;
            }

            int GetTokenCode()
            {
                if (IdnTable.GetCode(token) != 0)
                {
                    return TokenTable.GetCode("IDN");
                }
                else if (ConTable.GetCode(token) != 0)
                {
                    return TokenTable.GetCode("CON");
                }
                else
                {
                    return TokenTable.GetCode(token);
                }
            }

            if (TokenTable.Contains(token) || ConTable.Contains(token) || IdnTable.Contains(token))
            {
                Table.Add(new Token
                {
                    Code = number,
                    Row = numRow,
                    ValueType = Checker.Type,
                    Name = token,
                    IdnCode = IdnTable.GetCode(token),
                    ConCode = ConTable.GetCode(token),
                    TokenType = GetTokenType(),
                    TokenCode = GetTokenCode(),
                    Value = IdnTable.GetValue(token)
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
