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
    /// Interaction logic for InputWindow.xaml
    /// </summary>
    public partial class InputWindow : Window
    {
        public InputWindow(string idnName)
        {
            InitializeComponent();
            Title = $"Input value of '{idnName}':";
            Loaded += InputWindow_Loaded;
        }

        void InputWindow_Loaded(object sender, RoutedEventArgs e)
        {
            inputField.KeyDown += InputField_KeyDown;
            inputField.Focus();
        }

        void InputField_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.DialogResult = true;
            }
        }
    }
}
