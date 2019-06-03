using System.Collections.Generic;

namespace Translator_desktop.RPN.Generator
{
    public interface IPolishGenerator
    {
        List<string> Polish { get; }
        Dictionary<string, int> LabelsTable { get; }

        void Start();
    }
}
