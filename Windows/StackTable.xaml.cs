using System.Linq;
using System.Windows;

namespace Translator_desktop.Windows
{
    /// <summary>
    /// Interaction logic for StackTable.xaml
    /// </summary>
    public partial class StackTable : Window
    {
        public StackTable()
        {
            InitializeComponent();
            stackTable.ItemsSource = SyntaxAnalyse.OperatorPrecedenceMethod.Analyser.Table.Select(t => new { Step = t.step, t.Stack, t.Relation, t.InputString });
        }
    }
}
