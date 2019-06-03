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
    /// Interaction logic for SyntaxAnalyzerSettings.xaml
    /// </summary>
    public partial class SyntaxAnalyserSettings : Window
    {
        private MainWindow main;
        public string Algorithm {
            get
            {
                if ((bool)pushdownAutomaton.IsChecked)
                {
                    return pushdownAutomaton.Name;
                }
                else if ((bool)recursiveDescent.IsChecked)
                {
                    return recursiveDescent.Name;
                }
                else if ((bool)operatorPrecedence.IsChecked)
                {
                    return operatorPrecedence.Name;
                }
                return null;
            }}
        public SyntaxAnalyserSettings(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
            if (main.AnalyzeType.Equals("pushdownAutomaton"))
            {
                pushdownAutomaton.IsChecked = true;
            }
            else if(main.AnalyzeType.Equals("recursiveDescent"))
            {
                recursiveDescent.IsChecked = true;
            }
            else if (main.AnalyzeType.Equals("operatorPrecedence"))
            {
                operatorPrecedence.IsChecked = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            main.AnalyzeType = Algorithm;
            this.Close();
        }
    }
}
