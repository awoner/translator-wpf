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
    public partial class SyntaxAnalyzerSettings : Window
    {
        private MainWindow main;
        public string Algorithm {
            get
            {
                if ((bool)pushdownAutomation.IsChecked)
                {
                    return pushdownAutomation.Name;
                }
                else if((bool)recursiveDesedent.IsChecked)
                {
                    return recursiveDesedent.Name;
                }
                return null;
            }}
        public SyntaxAnalyzerSettings(MainWindow main)
        {
            InitializeComponent();
            this.main = main;
            if (main.AnalyzeType == "pushdownAutomation")
            {
                pushdownAutomation.IsChecked = true;
            }
            else
            {
                recursiveDesedent.IsChecked = true;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            main.AnalyzeType = Algorithm;
            this.Close();
        }
    }
}
