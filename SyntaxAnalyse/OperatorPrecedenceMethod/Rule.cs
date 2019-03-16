using System.Collections.Generic;

namespace Translator_desktop.SyntaxAnalyse.OperatorPrecedenceMethod
{
    public class Rule
    {
        public LinguisticUnit LeftPart { get; set; }
        public List<RightPart> RightParts { get; set; } = new List<RightPart>();
    }
}
