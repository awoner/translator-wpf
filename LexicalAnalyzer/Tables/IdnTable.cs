using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Translator_desktop.LexicalAnalyzer.Tables
{
    /// <summary>
    /// The class interprets a table of identifiers that contains all identifiers from the source code
    /// </summary>
    static class IdnTable
    {
        private static int code = 1;
        private static IList<Token> idnTable;
        public static IList<Token> Table { get => idnTable; }

        /// <summary>
        /// Return a string representation of the table
        /// </summary>
        public static string GetStringTable()
        {
            StringBuilder str = new StringBuilder("\tTable of Identifiers\n");
            string separator = new string('-', 35) + '\n';

            str.Append(separator);
            str.Append("|   №   |    Name    |    Type    |\n");
            str.Append(separator);

            foreach (Token token in idnTable)
            {
                str.AppendFormat("|{0, 4}   |{1, 8}    |{2, 8}    |\n", token.Code, token.Name, token.Type);
                str.Append(separator);
            }

            return str.ToString();
        }

        /// <summary>
        /// Initialization of tables of identifiers
        /// </summary>
        public static void InitTable()
        {
            idnTable = new List<Token>();
        }

        /// <summary>
        /// Get type of incoming token
        /// </summary>
        public static string GetType(string token)
        {
            string type = idnTable.FirstOrDefault(c => c.Name == token)?.Type;

            return (type != null) ? type : "undefined";

            /*
            foreach (Token idn in idnTable)
            {
                if (idn.Name == token)
                {
                    return idn.Type;
                }
            }

            return "undefined";
            */
        }

        /// <summary>
        /// Get identifier code
        /// </summary>
        public static int GetCode(string token)
        {
            int? code = idnTable.FirstOrDefault(c => c.Name == token)?.Code;

            return (code != null) ? (int)code : 0;
            /*
            foreach (Token idn in idnTable)
            {
                if (idn.Name == token)
                {
                    return idn.Code;
                }
            }

            return 0;
            */
        }

        /// <summary>
        /// Сhecks if the table of identifiers contains the incoming identifier
        /// </summary>
        public static bool Contains(string identifier)
        {
            return (idnTable.FirstOrDefault(c => c.Name == identifier) != null) ? true : false;

            /*
            foreach (Token idn in idnTable)
            {
                if (idn.Name == identifier)
                    return true;
            }
            return false;
            */
        }

        /// <summary>
        /// Add a token in the table
        /// </summary>
        public static void Add(string token)
        {
            idnTable.Add(new Token { Code = code, Name = token, Type = Checker.Type });
            code++;
        }
    }
}
