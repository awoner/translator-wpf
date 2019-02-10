using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translator_desktop.LexicalAnalyzer.Tables
{
    class Token
    {
        public int Code { get; set; }
        public int Row { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int IdnCode { get; set; }
        public int ConCode { get; set; }
        public int TokenCode { get; set; }
    }
}