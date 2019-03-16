using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Translator_desktop.LexicalAnalyse.Tables
{
    /// <summary>
    /// The class interprets a table of constants that contains all constants from the source code
    /// </summary>
    static class ConTable
    {
        private static int code = 1;
        private static IList<Token> conTable;
        public static IList<Token> Table { get => conTable; }

        /// <summary>
        /// Return a string representation of the table
        /// </summary>
        public static string GetStringTable()
        {
            StringBuilder str = new StringBuilder("\tTable of Constants\n");
            string separator = new string('-', 35) + '\n';

            str.Append(separator);
            str.Append("|   №   |    Name    |    Type    |\n");
            str.Append(separator);

            foreach (Token token in conTable)
            {
                str.AppendFormat("|{0, 4}   |{1, 9}   |{2, 8}    |\n", token.Code, token.Name, token.Type);
                str.Append(separator);
            }

            return str.ToString();
        }

        /// <summary>
        /// Initialization of tables of constants
        /// </summary>
        public static void InitTable()
        {
            conTable = new List<Token>();
        }

        /// <summary>
        /// Get constant code 
        /// </summary>
        public static int GetCode(string token)
        {
            int? code = conTable.FirstOrDefault(c => c.Name == token)?.Code;

            return code ?? 0;
        }

        /// <summary>
        /// Сhecks if the table of constants contains the incoming constant
        /// </summary>
        public static bool Contains(string constant)
        {
            return (conTable.FirstOrDefault(c => c.Name == constant) is null) ? false : true;
        }

        /// <summary>
        /// Add a token in the table
        /// </summary>
        public static void Add(string token, string type = null)
        {
            if (token.Contains('.') && (token.Contains('e') || token.Contains('E')) && type is null)
            {
                conTable.Add(new Token { Code = code, Name = token, Type = "double" });
                code++;
            }
            else if (token.Contains('.') && type is null)
            {
                conTable.Add(new Token { Code = code, Name = token, Type = "float" });
                code++;
            }
            else if (type is null)
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
