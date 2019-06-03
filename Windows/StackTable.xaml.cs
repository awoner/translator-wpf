using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Translator_desktop.LexicalAnalyse.Tables;
using Translator_desktop.RPN;
using Translator_desktop.RPN.Generator;

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
            stackTable.ItemsSource = DeijkstraGenerator.Table;
        }
    }
}
