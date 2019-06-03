using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Translator_desktop.SyntaxAnalyse.OperatorPrecedenceMethod;

namespace Translator_desktop.Windows
{
    /// <summary>
    /// Interaction logic for RelationshipsTableWindow.xaml
    /// </summary>
    ///
    public partial class RelationshipsTableWindow : Window
    {
        public RelationshipsTableWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var relationships = RelationshipsTable.RelationshipsTableBuffer;

            var dataTable = new DataTable();
            dataTable.Columns.Add("First\\Second", typeof(string));

            var rows = relationships.Select(rl => rl.FirstLinguisticUnit.Name).Distinct();
            var columns = relationships.Select(rl => rl.SecondLinguisticUnit.Name).Distinct();

            foreach (string secondLU in columns)
            {
                string newSecondLU = $"[{secondLU}]";
                dataTable.Columns.Add(newSecondLU, typeof(string));
            }

            foreach (string firstLU in rows)
            {
                DataRow row;
                row = dataTable.NewRow();

                foreach (var column in dataTable.Columns)
                {
                    row[column.ToString()] = column.ToString().Equals("First\\Second") 
                        ? firstLU
                        : relationships.FirstOrDefault(
                            rl => 
                                rl.FirstLinguisticUnit.Name.Equals(firstLU) 
                                && rl.SecondLinguisticUnit.Name.Equals(column.ToString().Replace("[", string.Empty).Replace("]", string.Empty))
                          )?.Relationship;
                }

                dataTable.Rows.Add(row);
            }

            foreach (DataColumn column in dataTable.Columns)
            {
                var gridColumn = new DataGridTextColumn()
                {
                    Header = column.ColumnName.Replace("[", string.Empty).Replace("]", string.Empty),
                    Binding = new Binding("[" + column.ColumnName + "]")
                };

                rlTable.Columns.Add(gridColumn);
            }

            foreach (DataRow row in dataTable.Rows)
            {
                rlTable.Items.Add(row);
            }
        }
    }
}
