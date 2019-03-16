using System.Windows;
using Translator_desktop.LexicalAnalyse.Tables;

namespace Translator_desktop.Pages
{
    /// <summary>
    /// Interaction logic for ConTableWindow.xaml
    /// </summary>
    public partial class ConTableWindow : Window
    {
        public ConTableWindow()
        {
            InitializeComponent();
        }

        private void conTable_Loaded(object sender, RoutedEventArgs e)
        {
            conTable.ItemsSource = ConTable.Table;
        }
    }
}
