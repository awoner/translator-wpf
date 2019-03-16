using System.Windows;
using Translator_desktop.SyntaxAnalyse.OperatorPrecedenceMethod;

namespace Translator_desktop.Windows
{
    /// <summary>
    /// Interaction logic for GrammarTableWindow.xaml
    /// </summary>
    public partial class GrammarTableWindow : Window
    {
        public GrammarTableWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            grammarTable.ItemsSource = RelationshipsTable.SimpleGrammar;
        }
    }
}
