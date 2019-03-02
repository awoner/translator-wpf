using System.Collections.Generic;

namespace Translator_desktop.SyntaxAnalyzer.PrecedenceRelationships
{
    public class Rule
    {
        public LinguisticUnit LeftPart { get; set; }
        public List<RightPart> RightParts { get; set; } = new List<RightPart>();
    }
}
