using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Translator_desktop.SyntaxAnalyzer.PrecedenceRelationships;

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
