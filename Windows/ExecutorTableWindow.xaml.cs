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
using Translator_desktop.RPN;

namespace Translator_desktop.Windows
{
    /// <summary>
    /// Interaction logic for ExecutorTableWindow.xaml
    /// </summary>
    public partial class ExecutorTableWindow : Window
    {
        public ExecutorTableWindow()
        {
            InitializeComponent();
            executorTable.ItemsSource = Executor.Table;
        }
    }
}
