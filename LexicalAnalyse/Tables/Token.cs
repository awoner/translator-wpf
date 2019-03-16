using System;

namespace Translator_desktop.LexicalAnalyse.Tables
{
    class Token : IEquatable<Token>
    {
        public int Code { get; set; }
        public int Row { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int IdnCode { get; set; }
        public int ConCode { get; set; }
        public int TokenCode { get; set; }

        public bool Equals(Token other)
        {
            return this.Code.Equals(other.Code) && this.Name.Equals(other.Name);
        }
    }
}