﻿using System;
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
    /// Interaction logic for PolisWindow.xaml
    /// </summary>
    public partial class PolisWindow : Window
    {
        public PolisWindow()
        {
            InitializeComponent();
            polisTable.ItemsSource = SyntaxAnalyse.OperatorPrecedenceMethod.Analyser.Table.Select(t => new { Step = t.step, t.Stack, t.Relation, t.InputString });
        }
    }
}
