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
using Translator_desktop.LexicalAnalyzer.Tables;

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
