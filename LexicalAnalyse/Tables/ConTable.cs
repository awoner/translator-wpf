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
        public static IList<Token> Table { get; private set; }

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

            foreach (Token token in Table)
            {
                str.AppendFormat("|{0, 4}   |{1, 9}   |{2, 8}    |\n", token.Code, token.Name, token.ValueType);
                str.Append(separator);
            }

            return str.ToString();
        }

        /// <summary>
        /// Initialization of tables of constants
        /// </summary>
        public static void InitTable()
        {
            Table = new List<Token>();
        }

        /// <summary>
        /// Get constant code 
        /// </summary>
        public static int GetCode(string token)
        {
            int? code = Table.FirstOrDefault(c => c.Name == token)?.Code;

            return code ?? 0;
        }

        /// <summary>
        /// Сhecks if the table of constants contains the incoming constant
        /// </summary>
        public static bool Contains(string constant)
        {
            return (Table.FirstOrDefault(c => c.Name == constant) is null) ? false : true;
        }

        /// <summary>
        /// Add a token in the table
        /// </summary>
        public static void Add(string token, string type = null)
        {
            if (token.Contains('.') && (token.Contains('e') || token.Contains('E')) && type is null)
            {
                Table.Add(new Token { Code = code, Name = token, ValueType = "double" });
                code++;
            }
            else if (token.Contains('.') && type is null)
            {
                Table.Add(new Token { Code = code, Name = token, ValueType = "float" });
                code++;
            }
            else if (type is null)
            {
                Table.Add(new Token { Code = code, Name = token, ValueType = "int" });
                code++;
            }
            else
            {
                Table.Add(new Token { Code = code, Name = token, ValueType = type });
                code++;
            }
        }
    }
}
