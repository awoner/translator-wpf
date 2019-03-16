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

namespace Translator_desktop.Windows
{
    /// <summary>
    /// Interaction logic for ParseTableWindow.xaml
    /// </summary>
    public partial class ParseTableWindow : Window
    {
        public ParseTableWindow()
        {
            InitializeComponent();
        }

        private void parseTable_Loaded(object sender, RoutedEventArgs e)
        {
            parseTable.ItemsSource = SyntaxAnalyse.PushdownAutomatonMethod.Analyser.GetTable();
        }
    }
}
