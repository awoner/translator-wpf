using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Documents;
using Translator_desktop.Pages;
using PushdownAutomatonMethod = Translator_desktop.SyntaxAnalyse.PushdownAutomatonMethod;
using OperatorPrecedenceMethod = Translator_desktop.SyntaxAnalyse.OperatorPrecedenceMethod;
using RecursiveDescentMethod = Translator_desktop.SyntaxAnalyse.RecursiveDescentMethod;
using Translator_desktop.Windows;
using Translator_desktop.RPN;
using Translator_desktop.RPN.Generator;

namespace Translator_desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string AnalyzeType { get; set; } = "recursiveDescent";

        public MainWindow()
        {
            InitializeComponent();
            TextRange doc = new TextRange(programCode.Document.ContentStart, programCode.Document.ContentEnd);
            using (FileStream fs = new FileStream($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\code.txt", FileMode.Open))
            {               
                    doc.Load(fs, DataFormats.Text);
            }
        }

        private void openFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";

            if (ofd.ShowDialog() == true)
            {
                TextRange doc = new TextRange(programCode.Document.ContentStart, programCode.Document.ContentEnd);
                using (FileStream fs = new FileStream(ofd.FileName, FileMode.Open))
                {
                    if (System.IO.Path.GetExtension(ofd.FileName).ToLower() == ".txt")
                        doc.Load(fs, DataFormats.Text);
                }
            }
        }

        private void saveFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Text Files (*.txt)|*.txt|All files (*.*)|*.*";
            if (sfd.ShowDialog() == true)
            {
                TextRange doc = new TextRange(programCode.Document.ContentStart, programCode.Document.ContentEnd);
                using (FileStream fs = File.Create(sfd.FileName))
                {
                    if (System.IO.Path.GetExtension(sfd.FileName).ToLower() == ".txt")
                        doc.Save(fs, DataFormats.Text);
                }
            }
        }

        private void exitFile_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            LexicalAnalyse.Analyser lexicalAnalyser = new LexicalAnalyse.Analyser();
            lexicalAnalyser.Parse(GetTextFromRichTextbox());
            
            try
            {
                if (AnalyzeType.Equals("pushdownAutomation"))
                {
                    PushdownAutomatonMethod.Analyser analyser = new PushdownAutomatonMethod.Analyser();
                    analyser.Parse();
                }
                else if (AnalyzeType.Equals("recursiveDescent"))
                {
                    RecursiveDescentMethod.Analyser analyser = new RecursiveDescentMethod.Analyser();

                    if (analyser.Parse())
                    {
                        listViewErrors.ItemsSource = null;

                        IPolishGenerator generator = new DeijkstraGenerator();
                        generator.Start();

                        ConsoleWindow consoleWindow = new ConsoleWindow();
                        consoleWindow.Show();

                        Executor polishExecutor = new Executor(generator, consoleWindow);
                        polishExecutor.Execute();                        
                    }
                    else
                    {
                        listViewErrors.ItemsSource = analyser.GetErrors();
                    }
                }
                else if (AnalyzeType.Equals("operatorPrecedence"))
                {
                    OperatorPrecedenceMethod.Analyser analyser = new OperatorPrecedenceMethod.Analyser();
                    analyser.Parse();
                    MessageBox.Show("Build successfully.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string[] GetTextFromRichTextbox()
        {
            TextRange textRange = new TextRange(programCode.Document.ContentStart, programCode.Document.ContentEnd);

            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(textRange.Text));

            ms.Position = 0;
            List<string> code = new List<string>();
            using (var reader = new StreamReader(ms, Encoding.ASCII))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    code.Add(line);
                }

                //code[0] = "$ " + code[0];
                
                //code[code.Count - 1] += " $";
            }

            return code.ToArray();
        }

        private void outputTokenTablePage_Click(object sender, RoutedEventArgs e)
        {
            OutputTokenTableWindow outputTokenTableWindow = new OutputTokenTableWindow();
            outputTokenTableWindow.Show();
        }

        private void idnTablePage_Click(object sender, RoutedEventArgs e)
        {
            IdnTableWindow idnTableWindow = new IdnTableWindow();
            idnTableWindow.Show();
        }

        private void conTablePage_Click(object sender, RoutedEventArgs e)
        {
            ConTableWindow conTableWindow = new ConTableWindow();
            conTableWindow.Show();
        }

        private void parseTablePage_Click(object sender, RoutedEventArgs e)
        {
            ParseTableWindow parseTableWindow = new ParseTableWindow();
            parseTableWindow.Show();
        }

        private void syntaxSettings_Click(object sender, RoutedEventArgs e)
        {
            SyntaxAnalyserSettings syntaxAnalyserSettings = new SyntaxAnalyserSettings(this);
            syntaxAnalyserSettings.Show();
            AnalyzeType = syntaxAnalyserSettings.Algorithm;
        }

        private void grammarTable_Click(object sender, RoutedEventArgs e)
        {
            GrammarTableWindow grammarTableWindow = new GrammarTableWindow();
            grammarTableWindow.Show();
        }

        private void relationshipsTable_Click(object sender, RoutedEventArgs e)
        {
            RelationshipsTableWindow relationshipsTableWindow = new RelationshipsTableWindow();
            relationshipsTableWindow.Show();
        }

        private void StackTable_Click(object sender, RoutedEventArgs e)
        {
            StackTable stackTableWindow = new StackTable();
            stackTableWindow.Show();
        }

        private void CalculatorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            CalculatorWindow calculatorWindow = new CalculatorWindow();
            calculatorWindow.Show();
        }

        private void ExecutorTable_Click(object sender, RoutedEventArgs e)
        {
            ExecutorTableWindow executorTableWindow = new ExecutorTableWindow();
            executorTableWindow.Show();
        }
    }
}
