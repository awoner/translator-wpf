using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Translator_desktop.LexicalAnalyse.Tables;
using Translator_desktop.SyntaxAnalyse.OperatorPrecedenceMethod;
using OperatorPrecedenceMethod = Translator_desktop.SyntaxAnalyse.OperatorPrecedenceMethod;

namespace Translator_desktop.Windows
{
    /// <summary>
    /// Interaction logic for CalculatorWindow.xaml
    /// </summary>
    public partial class CalculatorWindow : Window
    {
        public class Variable
        {
            public string Name { get; set; }
            public double Value { get; set; }
        }
        private ObservableCollection<Variable> variablesList = new ObservableCollection<Variable>();

        public CalculatorWindow()
        {
            InitializeComponent();
            variablesListView.ItemsSource = variablesList;
        }

        private void AddVariableButton_Click(object sender, RoutedEventArgs e)
        {
            variablesList.Add(new Variable());
            //variablesListView.Items.Refresh();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            resultTextBox.IsReadOnly = false;
            errorsTextBox.IsReadOnly = false;
            try
            {
                if (!string.IsNullOrEmpty(expressionTextBox.Text))
                {
                    LexicalAnalyse.Analyser lexicalAnalyser = new LexicalAnalyse.Analyser();
                    if (variablesListView.Items.Count > 1)
                    {
                        for (int i = 0; i < variablesListView.Items.Count - 1; i++)
                        {
                            var ci = new DataGridCellInfo(variablesListView.Items[i], variablesListView.Columns[0]);
                            var varName = ((Variable)ci.Item).Name;

                            ci = new DataGridCellInfo(variablesListView.Items[i], variablesListView.Columns[1]);
                            var varValue = ((Variable)ci.Item).Value.ToString();

                            IdnTable.Add(varName, Double.Parse(varValue));
                        }
                    }

                    if (lexicalAnalyser.Parse(new string[] { "$ " + expressionTextBox.Text.Trim() + " $" }))
                    {
                        RelationshipsTable.InitTable();
                        OperatorPrecedenceMethod.Analyser syntaxAnalyser = new OperatorPrecedenceMethod.Analyser();
                        syntaxAnalyser.Parse();

                        resultTextBox.Text = syntaxAnalyser.GetPolisResult().ToString();
                        errorsTextBox.Text = syntaxAnalyser.Error;
                    }
                    else
                    {
                        errorsTextBox.Text = "Lexical error!";
                    }
                    resultTextBox.IsReadOnly = true;
                    errorsTextBox.IsReadOnly = true;
                }
                else
                {
                    errorsTextBox.Text = "Error! Expression can't be null or empty. Please fill expression form.";
                    errorsTextBox.IsReadOnly = true;
                    MessageBox.Show("Error!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"ERROR! {ex.Message}");
            }
        }

        private void VariablesListView_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            
        }
    }
}
