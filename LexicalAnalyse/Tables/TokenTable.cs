using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Translator_desktop.LexicalAnalyse.Tables
{
    /// <summary>
    /// The class interprets a table of tokens
    /// </summary>
    static class TokenTable
    {
        private static string[] tokens =
        {
            "double", "float", "int", "for", "if",
            "cin", "cout", "{", "}", ";", ",", "=",
            "<<", ">>", "<", ">", "<=", ">=", "==",
            "!=", "+", "-", "*", "/", "(", ")", "#",
            "$", "true", "false", "IDN", "CON"
        };
        public static IList<Token> Table { get; private set; }

        /// <summary>
        /// Initialization of tables of tokens
        /// </summary>
        public static void InitTable()
        {
            Table = new List<Token>();

            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] == "IDN")
                {
                    Table.Add(new Token { Code = 100, Name = tokens[i] });
                }
                else if (tokens[i] == "CON")
                {
                    Table.Add(new Token { Code = 101, Name = tokens[i] });
                }
                else
                {
                    Table.Add(new Token { Code = i + 1, Name = tokens[i] });
                }
            }
        }

        /// <summary>
        /// Return a string representation of the table
        /// </summary>
        public static string GetStringTable()
        {
            StringBuilder str = new StringBuilder("Table of Tokens");
            string separator = new string('-', 25) + '\n';

            str.Append(separator);
            str.Append("|   Code   |    Name    |\n");
            str.Append(separator);

            foreach (Token token in Table)
            {
                str.AppendFormat("|{0, 6}    |{1, 9}   |\n", token.Code, token.Name);
                str.Append(separator);
            }

            return str.ToString();
        }

        /// <summary>
        /// Get token code
        /// </summary>
        public static int GetCode(string token)
        {
            int? code = Table.FirstOrDefault(c => c.Name == token)?.Code;

            return code ?? 0;
        }

        /// <summary>
        /// Сhecks if the table of tokens contains the incoming token
        /// </summary>
        public static bool Contains(string token)
        {
            return (Table.FirstOrDefault(c => c.Name == token) is null) ? false : true;
        }
    }
}
