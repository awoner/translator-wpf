using System.Collections.Generic;

namespace Translator_desktop.SyntaxAnalyzer
{
    public class Row
    {
        public string InputToken { get; set; }
        public int State { get; set; }
        public string StackState { get; set; }
    }
}