namespace Translator_desktop.SyntaxAnalyse.OperatorPrecedenceMethod
{
    public class RelationshipToken
    {
        public LinguisticUnit FirstLinguisticUnit { get; set; }
        public LinguisticUnit SecondLinguisticUnit { get; set; }
        public string Relationship { get; set; }
    }
}