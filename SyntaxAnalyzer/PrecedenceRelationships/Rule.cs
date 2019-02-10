using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translator_desktop.SyntaxAnalyzer.PrecedenceRelationships
{
    public class Rule
    {
        public LinguisticUnit LeftPart { get; set; }
        public List<RightPart> RightParts { get; set; } = new List<RightPart>();
    }
}
