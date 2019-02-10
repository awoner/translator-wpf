using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
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
using Translator_desktop.SyntaxAnalyzer.PrecedenceRelationships;

namespace Translator_desktop.Windows
{
    /// <summary>
    /// Interaction logic for RelationshipsTableWindow.xaml
    /// </summary>
    /// 
    public class MatrixToDataViewConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var myDataTable = new DataTable();
            var colums = values[0] as string[];
            var rows = values[1] as string[];
            var vals = values[2] as string[][];
            myDataTable.Columns.Add("---");    //The blanc corner column
            foreach (var value in colums)
            {
                myDataTable.Columns.Add(value);
            }
            int index = 0;

            foreach (string row in rows)
            {
                var tmp = new string[1 + vals[index].Count()];
                vals[index].CopyTo(tmp, 1);
                tmp[0] = row;
                myDataTable.Rows.Add(tmp);
                index++;
            }
            return myDataTable.DefaultView;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    public partial class RelationshipsTableWindow : Window
    {
        public RelationshipsTableWindow()
        {

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var relationships = RelationshipsTable.relationshipsTable;
            string str = "";


            for (int i = 0; i < relationships.Count; i++)
            {
                str += $"{relationships[i].FirstLinguisticUnit.Name}\t{relationships[i].Relationship}\t{relationships[i].SecondLinguisticUnit.Name}\n";
         
            }

            File.WriteAllText(@"C:\Users\lesha\Desktop\conflicts.txt", str);

            //DataTable dataTable = new DataTable();
            //List<string> relationshipTokens = new List<string>();
            //dataTable.Columns.Add("---");
            //foreach (var relation in relationships)
            //{
            //    if(!dataTable.Columns.Contains(relation.SecondLinguisticUnit.Name))
            //        dataTable.Columns.Add(relation.SecondLinguisticUnit.Name.ToString());
               
            //}

            ////foreach (var item in relationships)
            ////{
            ////    relationshipTokens.Add(item.FirstLinguisticUnit.Name);
            ////    relationshipTokens.Add(item.Relationship);
            ////}
            ////int j = 0;

            //for (int j = 0; j < relationships.Count; j++)
            //{
            //    var newRow = dataTable.NewRow();
            //    for (int i = 0; i < dataTable.Columns.Count; i++)
            //    {
            //        if(i == 0)
            //            newRow[dataTable.Columns[i].ColumnName] = relationships[i].FirstLinguisticUnit.Name.ToString();
            //        newRow[dataTable.Columns[i].ColumnName] = relationships[i].Relationship.ToString();
            //    }
            //    dataTable.Rows.Add(newRow);
            //}

            ////dataTable.Rows.Add(relationshipTokens);
            //rlTable.DataContext = dataTable.DefaultView;
            
        }
    }
}
