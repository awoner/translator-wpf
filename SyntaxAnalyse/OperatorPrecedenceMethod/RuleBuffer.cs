namespace Translator_desktop.SyntaxAnalyse.OperatorPrecedenceMethod
{
    public class RuleBuffer
    {
        public string LeftPart { get; set; }
        public string RightPart { get; set; }
        public string Equal { get; set; } = "::=";
    }
}
