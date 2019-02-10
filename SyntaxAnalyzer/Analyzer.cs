using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using Translator_desktop.LexicalAnalyzer.Tables;

namespace Translator_desktop.SyntaxAnalyzer
{
    public class Analyzer
    {
        private static List<State> conversionTable;           //таблица переходов
        private static IList<Token> outputTokenTable;         //выходная таблица лексем
        private static Stack<int?> stackState;                //стек состояний
        private static List<Row> parseTable;                  //таблица разбора

        public static List<Row> GetTable()
        {
            return parseTable;
        }

        public Analyzer()
        {
            conversionTable = new List<State>();
            outputTokenTable = OutputTokenTable.GetTable();
            stackState = new Stack<int?>();
            parseTable = new List<Row>();

            try
            {
                string path = @"C:\Users\lesha\source\repos\Translator_desktop\Translator_desktop\SyntaxAnalyzer\ConversionTable.json";

                using (StreamReader file = File.OpenText(path))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    conversionTable = (List<State>)serializer.Deserialize(file, typeof(List<State>));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string GetLabelOfToken(string token)
        {
            if (IdnTable.Contains(token))
            {
                return "id";
            }
            else if (ConTable.Contains(token))
            {
                return "con";
            }
            else
            {
                return token;
            }
        }

        public void Parse()
        {
            int i = 0;
            int current_state = 1;
            bool end = false;
            List<State> stateConversions;
            State currentConversion;

            while (!end)
            {
                if (i <= outputTokenTable.Count)
                {
                    stateConversions = conversionTable.FindAll(s => s.CurrentState == current_state);     //все переходы которые можно выполнить из текущего состояния
                    currentConversion = stateConversions.FirstOrDefault(x => x.Label == (i < outputTokenTable.Count ? GetLabelOfToken(outputTokenTable[i].Name) : ""));  //переход для текущей лексемы

                    if (stateConversions.First().SemanticSubroutine.Contains("[=]exit"))
                    {
                        if (stackState.Count != 0)
                        {
                            current_state = stackState?.Pop() ?? current_state;
                        }
                        else
                        {
                            end = true;
                            MessageBox.Show("Build successfully.");
                        }
                    }
                    else if (currentConversion != null)  //если переход возможен, то
                    {
                        if (currentConversion.StateStack != null)
                        {
                            stackState.Push(currentConversion.StateStack);  //заносим в стек
                        }
                        parseTable.Add(new Row { State = current_state, InputToken = outputTokenTable[i].Name, StackState = string.Join(",", stackState)});  //добавляеться запись в таблицу разбора
                        current_state = currentConversion.NextState ?? (int)(stackState?.Pop());    //текущему состоянию присваиваеться следуюющее состояние за текущим переходом
                        i++;    //переход на следующую лексему
                    }
                    else
                    {
                        var numRegex = new Regex(@"\d+");
                        var currentStateNotEqual = stateConversions.First(x => x.CurrentState == current_state);
                        if (currentStateNotEqual.SemanticSubroutine.Contains("[!=]error"))
                        {
                            string error = "";
                            foreach (var state in stateConversions)
                            {
                                error += state.Label + " ";
                            }
                            MessageBox.Show($"ERROR! Expected {error.TrimEnd().Replace(" ", " or ")}.");
                            end = true;
                        }
                        else if (currentStateNotEqual.SemanticSubroutine.Contains("[!=]exit"))
                        {
                            if (stackState.Count != 0)
                            {
                                current_state = stackState?.Pop() ?? current_state;
                            }
                            else
                            {
                                end = true;
                                MessageBox.Show("Build successfully.");
                            }
                            //if (stackState.Count != 0)
                            //{
                            //    current_state = stackState?.Pop() ?? current_state;
                            //}
                        }
                        else if (numRegex.IsMatch(currentStateNotEqual.SemanticSubroutine))//currentStateNotEqual.SemanticSubroutine.Contains("[!=]<s/a op.>")
                        {
                            var matches = numRegex.Matches(currentStateNotEqual.SemanticSubroutine);
                            stackState.Push(int.Parse(matches[1].ToString()));
                            current_state = int.Parse(matches[0].ToString());
                        }
                        //else if (stateConversions.First().SemanticSubroutine.Contains("[!=]<s/a E>"))//
                        //{
                        //    int state;
                        //    int.TryParse(string.Join("", currentStateNotEqual.SemanticSubroutine.Where(c => char.IsDigit(c))), out state);
                        //    stackState.Push(state);
                        //    current_state = 30;
                        //}
                    }
                }
                else
                {
                    end = true;
                }
            }
        }
    }
}