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

namespace Translator_desktop.Windows
{
    /// <summary>
    /// Interaction logic for IdnTableWindow.xaml
    /// </summary>
    public partial class IdnTableWindow : Window
    {
        public IdnTableWindow()
        {
            InitializeComponent();
        }

        private void idnTable_Loaded(object sender, RoutedEventArgs e)
        {
            idnTable.ItemsSource = IdnTable.GetTable();
        }
    }
}
