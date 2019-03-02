using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Translator_desktop.LexicalAnalyzer.Tables
{
    /// <summary>
    /// The class interprets a table of tokens
    /// </summary>
    static class TokenTable
    {
        private static string[] tokens = { "double", "float", "int", /*"bool" ,*/ "for", "if",
            "cin", "cout", "{", "}", ";", ",", "=", "<<", ">>", "<", ">", "<=",
            ">=", "==", "!=", "+", "-", "*", "/", "(", ")", "true", "false", "IDN", "CON" };
        private static IList<Token> tokenTable = new List<Token>();

        /// <summary>
        /// Initialization of tables of tokens
        /// </summary>
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

            foreach (Token token in tokenTable)
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
            int? code = tokenTable.FirstOrDefault(c => c.Name == token)?.Code;

            return (code != null) ? (int)code : 0;

            /*
            foreach (Token t in tokenTable)
            {
                if (t.Name == token)
                {
                    return t.Code;
                }
            }

            return 0;
            */
        }

        /// <summary>
        /// Сhecks if the table of tokens contains the incoming token
        /// </summary>
        public static bool Contains(string token)
        {
            return (tokenTable.FirstOrDefault(c => c.Name == token) != null) ? true : false;

            /*
            foreach (Token t in tokenTable)
            {
                if (t.Name == token)
                {
                    return true;
                }
            }
            return false;
            */
        }
    }
}
