using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using Translator_desktop.Pages;
using Translator_desktop.SyntaxAnalyser;
using Translator_desktop.SyntaxAnalyzer.PrecedenceRelationships;
using Translator_desktop.Windows;

namespace Translator_desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string AnalyzeType { get; set; } = "pushdownAutomation";

        public MainWindow()
        {
            InitializeComponent();
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            LexicalAnalyzer.Analyzer lexicalAnalyzer = new LexicalAnalyzer.Analyzer();
            
            lexicalAnalyzer.Parse(GetTextFromRichTextbox());


            RelationshipsTable relationshipsTable = new RelationshipsTable();


            if (AnalyzeType == "pushdownAutomation")
            {
                SyntaxAnalyzer.Analyzer analyzer = new SyntaxAnalyzer.Analyzer();
                analyzer.Parse();
            }
            else
            {
                RecursiveDescent recursiveDescent = new RecursiveDescent();

                if (recursiveDescent.Parse())
                {
                    listViewErrors.ItemsSource = null;
                    MessageBox.Show("Build successfully.");
                }
                else
                {
                    listViewErrors.ItemsSource = recursiveDescent.GetErrors();
                }
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
            SyntaxAnalyzerSettings syntaxAnalyzerSettings = new SyntaxAnalyzerSettings(this);
            syntaxAnalyzerSettings.Show();
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
    }
}
