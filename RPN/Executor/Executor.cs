using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Translator_desktop.LexicalAnalyse.Tables;
using Translator_desktop.RPN.Generator;
using Translator_desktop.Windows;

namespace Translator_desktop.RPN
{
    public class Executor
    {
        public static List<object> Table { get; private set; }
        private IPolishGenerator Generator { get; set; }
        private readonly ConsoleWindow consoleWindow;
        private Dictionary<string, double> cycleDesignations;
        private Stack<string> stack;

        public Executor(IPolishGenerator generator, ConsoleWindow consoleWindow)
        {
            Table = new List<object>();
            stack = new Stack<string>();
            cycleDesignations = new Dictionary<string, double>();
            Generator = generator;
            this.consoleWindow = consoleWindow;
        }

        public void Execute()
        {
            double secondOperand, firstOperand;
            string @operator, lastLabel = string.Empty;
            bool jumpFlag = false;
            Regex labelRegex = new Regex("l\\d+");

            for (int i = 0; i < Generator.Polish.Count; i++)
            {
                string token = Generator.Polish[i];

                if (jumpFlag)
                {
                    if (token == lastLabel)
                    {
                        jumpFlag = false;
                    }
                    continue;
                }

                if ((token != "true" && token != "false") && (Checker.IsUnaryOperator(token) || Checker.IsBinaryOperator(token)))
                {
                    if (Checker.IsBinaryOperator(token))
                    {
                        @operator = token;

                        if (Checker.IsOperator(@operator))
                        {
                            secondOperand = GetOperand();
                            firstOperand = GetOperand();

                            ExecuteExpression(firstOperand, secondOperand, @operator);
                            AddToViewTable(token, $"{firstOperand} {@operator} {secondOperand}");
                        }
                        else if (Checker.IsRelation(@operator))
                        {
                            secondOperand = GetOperand();
                            firstOperand = GetOperand();

                            ExecuteRelation(firstOperand, secondOperand, @operator);
                            AddToViewTable(token, $"{firstOperand} {@operator} {secondOperand}");
                        }
                        else if (Checker.IsIO(@operator))
                        {
                            ExecuteIO(stack.Pop(), @operator);
                        }
                        else if (@operator == "=")
                        {
                            secondOperand = GetOperand();
                            string identifierName = stack.Pop();

                            if (new Regex("r\\d+").IsMatch(identifierName))
                            {
                                if (cycleDesignations.FirstOrDefault(r => r.Key == identifierName).Key is null)
                                {
                                    cycleDesignations.Add(identifierName, secondOperand);
                                }
                                else
                                {
                                    cycleDesignations[identifierName] = secondOperand;
                                }
                            }
                            else
                            {
                                IdnTable.SetValue(identifierName, secondOperand);
                            }
                            AddToViewTable(token, $"{identifierName} {@operator} {secondOperand}");
                        }
                    }
                    else if (Checker.IsUnaryOperator(token))
                    {
                        if (token == "@")
                        {
                            string operand = stack.Pop();

                            if (IdnTable.Contains(operand) && IdnTable.GetValue(operand).HasValue)
                            {
                                IdnTable.SetValue(operand, -IdnTable.GetValue(operand).Value);
                            }

                            stack.Push($"-{operand}");
                            AddToViewTable(token, $"Change sign of {operand}");
                        }
                        else
                        {
                            string identifier;
                            while (stack.Count > 0)
                            {
                                identifier = stack.Pop();

                                if (IdnTable.GetType(identifier) != token)
                                {
                                    IdnTable.SetType(identifier, token);
                                }
                            }
                        }
                    }
                }
                else if (token.Contains("JNE"))
                {
                    lastLabel = $"{labelRegex.Match(token).Groups[0].Value}:";
                    bool condition = Boolean.Parse(stack.Pop());

                    jumpFlag = condition ? false : true;
                    AddToViewTable(token, $"Jump to label {lastLabel} if last condition is false.");
                }
                else if (token.Contains("JMP"))
                {
                    string label = labelRegex.Match(token).Groups[0].Value;

                    i = Generator.LabelsTable[label];
                    AddToViewTable(token, $"Jump to label {label}:");

                }
                else if(!token.Contains(":"))
                {
                    stack.Push(token);
                    AddToViewTable(token, "Add input token to stack");
                }
            }
        }

        private double GetOperand()
        {
            if (IdnTable.Contains(stack.Peek()))
            {
                return (double)IdnTable.GetValue(stack.Pop());
            }
            else if (Double.TryParse(stack.Peek(), NumberStyles.Any, CultureInfo.InvariantCulture, out double operand))
            {
                stack.Pop();
                return operand;
            }
            else if (new Regex("r\\d+").IsMatch(stack.Peek()))
            {
                return cycleDesignations[stack.Pop()];
            }

            throw new NotImplementedException();
        }

        private void ExecuteRelation(double firstOperand, double secondOperand, string @operator)
        {
            switch (@operator)
            {
                case ">":
                    stack.Push((firstOperand > secondOperand).ToString().ToLower());
                    break;
                case "<":
                    stack.Push((firstOperand < secondOperand).ToString().ToLower());
                    break;
                case ">=":
                    stack.Push((firstOperand >= secondOperand).ToString().ToLower());
                    break;                                               
                case "<=":                                               
                    stack.Push((firstOperand <= secondOperand).ToString().ToLower());
                    break;                                               
                case "==":                                               
                    stack.Push((firstOperand == secondOperand).ToString().ToLower());
                    break;                                               
                case "!=":                                               
                    stack.Push((firstOperand != secondOperand).ToString().ToLower());
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void ExecuteExpression(double firstOperand, double secondOperand, string @operator)
        {
            switch (@operator)
            {
                case "+":
                    stack.Push((firstOperand + secondOperand).ToString().Replace(",", "."));
                    break;
                case "-":
                    stack.Push((firstOperand - secondOperand).ToString().Replace(",", "."));
                    break;
                case "/":
                    stack.Push((firstOperand / secondOperand).ToString().Replace(",", "."));
                    break;
                case "*":
                    stack.Push((firstOperand * secondOperand).ToString().Replace(",", "."));
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void ExecuteIO(string operand, string @operator)
        {
            switch (@operator)
            {
                case ">>":
                    string outputValue = string.Empty;
                    if (IdnTable.Contains(operand))
                    {
                        outputValue = IdnTable.GetValue(operand).ToString();
                    }
                    else
                    {
                        outputValue = "undefined";
                    }
                    consoleWindow.consoleContent.ConsoleOutput.Add(outputValue);             
                    break;
                case "<<":
                    consoleWindow.Focus();

                    var inputWindow = new InputWindow(operand);

                    inputWindow.ShowDialog();

                    if (Double.TryParse(inputWindow.inputField.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
                    {
                        IdnTable.SetValue(operand, value);
                    }
                    else
                    {
                        MessageBox.Show("Invalid value!");
                    }

                    break;
                default:
                    throw new InvalidOperationException();
            }
        }

        private void AddToViewTable(string token, string action)
        {
            Table.Add(new { InputString = token, Stack = string.Join(" ", stack.Reverse()), Action = action });
        }
    }
}
