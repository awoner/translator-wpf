using System;

namespace Translator_desktop.LexicalAnalyse
{
    public class Token : IEquatable<Token>, ICloneable
    {
        public int Code { get; set; }
        public int Row { get; set; }
        public string Name { get; set; }
        public double? Value { get; set; }
        public string ValueType { get; set; }
        public string TokenType { get; set; }
        public int IdnCode { get; set; }
        public int ConCode { get; set; }
        public int TokenCode { get; set; }

        public int GetPriority()
        {
            switch (TokenType.Equals("@") ? TokenType : Name)
            {
                case "if":
                case "for":
                case "}":
                    return 0;

                case "cin":
                case "cout":
                    return 1;

                case "int":
                case "float":
                case "double":
                case "{":
                case "=":
                case ";":
                case "<<":
                case ">>":
                    return 2;

                case ",":
                case "(":
                    return 3;

                case ")":
                    return 4;

                case ">":
                case "<":
                case ">=":
                case "<=":
                case "!=":
                case "==":
                case "true":
                case "false":
                    return 5;

                case "+":
                case "-":
                    return 6;

                case "@":
                case "*":
                case "/":
                    return 7;

                default:
                    throw new InvalidOperationException();
            }
        }

        public object Clone()
        {
            return
                new Token
                {
                    Code = Code,
                    Row = Row,
                    Name = Name,
                    Value = Value,
                    ValueType = ValueType,
                    TokenType = TokenType,
                    IdnCode = IdnCode,
                    ConCode = ConCode,
                    TokenCode = TokenCode
                };
        }

        public bool Equals(Token other)
        {
            return Code == other.Code && Name == other.Name;
        }
    }
}