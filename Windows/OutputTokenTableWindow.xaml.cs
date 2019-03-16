using System.Windows;
using Translator_desktop.LexicalAnalyse.Tables;

namespace Translator_desktop.Windows
{
    /// <summary>
    /// Interaction logic for OutputTokenTableWindow.xaml
    /// </summary>
    public partial class OutputTokenTableWindow : Window
    {
        public OutputTokenTableWindow()
        {
            InitializeComponent();
        }

        private void outputTokenTable_Loaded(object sender, RoutedEventArgs e)
        {
            outputTokenTable.ItemsSource = OutputTokenTable.Table;
        }
    }
}
