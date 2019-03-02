namespace Translator_desktop.SyntaxAnalyzer.PrecedenceRelationships
{
    public class RuleBuffer
    {
        public string LeftPart { get; set; }
        public string RightPart { get; set; }
        public string Equal { get; set; } = "::=";
    }
}
