﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Translator_desktop.LexicalAnalyse;
using Translator_desktop.LexicalAnalyse.Tables;

namespace Translator_desktop.SyntaxAnalyse.PushdownAutomatonMethod
{
    public class Analyser
    {
        private static List<State> conversionTable;
        private static IList<Token> outputTokenTable;
        private static Stack<int?> stackState;
        private static List<Row> parseTable;

        public static List<Row> GetTable()
        {
            return parseTable;
        }

        public Analyser()
        {
            conversionTable = new List<State>();
            outputTokenTable = OutputTokenTable.Table.ToList();
            stackState = new Stack<int?>();
            parseTable = new List<Row>();

            try
            {
                string projectRoot = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                string path = Directory.GetFiles(projectRoot, "ConversionTable.json", SearchOption.AllDirectories).FirstOrDefault();

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
                    //all transitions that can be performed from the current state
                    stateConversions = conversionTable.FindAll(s => s.CurrentState == current_state);

                    //jump to the current token
                    currentConversion = stateConversions.FirstOrDefault(x => x.Label == (i < outputTokenTable.Count ? GetLabelOfToken(outputTokenTable[i].Name) : ""));

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
                    //else if transfer is possible
                    else if (currentConversion != null)
                    {
                        if (currentConversion.StateStack != null)
                        {
                            //put on the stack
                            stackState.Push(currentConversion.StateStack);
                        }

                        //adds an entry to the parse table
                        parseTable.Add(new Row { State = current_state, InputToken = outputTokenTable[i].Name, StackState = string.Join(",", stackState)});

                        //the current state is assigned the following state after the current transition
                        current_state = currentConversion.NextState ?? (int)(stackState?.Pop());

                        //move to the next token
                        i++;
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