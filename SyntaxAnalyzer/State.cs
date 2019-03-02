namespace Translator_desktop.SyntaxAnalyzer
{
    public class State
    {
        public int CurrentState { get; set; }
        public string Label { get; set; }
        public int? StateStack { get; set; }
        public int? NextState { get; set; }
        public string SemanticSubroutine { get; set; }
    }
}
