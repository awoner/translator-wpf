using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;

namespace Translator_desktop.LexicalAnalyse.Tables
{
    /// <summary>
    /// The class interprets a table of identifiers that contains all identifiers from the source code
    /// </summary>
    static class IdnTable
    {
        private static int code = 1;
        public static IList<Token> Table { get; private set; }

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

            foreach (Token token in Table)
            {
                str.AppendFormat("|{0, 4}   |{1, 8}    |{2, 8}    |\n", token.Code, token.Name, token.ValueType);
                str.Append(separator);
            }

            return str.ToString();
        }

        /// <summary>
        /// Initialization of tables of identifiers
        /// </summary>
        public static void InitTable()
        {
            Table = new List<Token>();
        }

        /// <summary>
        /// Get type of incoming token
        /// </summary>
        public static string GetType(string token)
        {
            string type = Table.FirstOrDefault(c => c.Name == token)?.ValueType;

            return type ?? "undefined";
        }

        internal static void SetType(string identifierName, string typeName)
        {
            Table.First(idn => idn.Name == identifierName).ValueType = typeName;
        }

        /// <summary>
        /// Get identifier code
        /// </summary>
        public static int GetCode(string token)
        {
            int? code = Table.FirstOrDefault(c => c.Name == token)?.Code;

            return code ?? 0;
        }

        /// <summary>
        /// Сhecks if the table of identifiers contains the incoming identifier
        /// </summary>
        public static bool Contains(string identifier)
        {
            return (Table.FirstOrDefault(c => c.Name == identifier) is null) ? false : true;
        }

        public static double? GetValue(string identifier)
        {
            return Table.FirstOrDefault(idn => idn.Name.Equals(identifier))?.Value;
        }

        public static void SetValue(string identifierName, double value)
        {
            if (Contains(identifierName))
            {
                string type = Table.First(idn => idn.Name == identifierName).ValueType;
                switch (type)
                {
                    case "int":
                        Table.First(idn => idn.Name == identifierName).Value = (int)value;
                        break;
                    case "float":
                        Table.First(idn => idn.Name == identifierName).Value = Math.Round((float)value, 4);
                        break;
                    case "double":
                        Table.First(idn => idn.Name == identifierName).Value = value;
                        break;
                    default:
                        throw new InvalidCastException();
                }
                
            }
            else
            {
                throw new Exception($"Identifier {identifierName} does not exist!");
            }
        }

        /// <summary>
        /// Add a token in the table
        /// </summary>
        public static void Add(string token)
        {
            Table.Add(new Token { Code = code, Name = token, ValueType = Checker.Type });
            code++;
        }

        public static void Add(string token, double value)
        {
            Table.Add(new Token { Code = code, Name = token, Value = value, ValueType = Checker.Type });
            code++;
        }
    }
}
