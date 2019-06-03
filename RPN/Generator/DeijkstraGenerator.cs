using System.Collections.Generic;
using System.Linq;
using Translator_desktop.LexicalAnalyse;
using Translator_desktop.LexicalAnalyse.Tables;

namespace Translator_desktop.RPN.Generator
{
    public class DeijkstraGenerator : IPolishGenerator
    {
        public List<string> Polish { get; private set; }
        public Dictionary<string, int> LabelsTable { get; private set; }
        public static List<object> Table { get; private set; }
        private readonly List<Token> outputTokenTable;
        private Stack<Token> stack;
        private Stack<string> labelsStack;
        private int labelCounter = 1;
        private int cycleDesignationCounter = 0;

        public DeijkstraGenerator()
        {
            Polish = new List<string>();
            LabelsTable = new Dictionary<string, int>();
            outputTokenTable = OutputTokenTable.Table as List<Token>;
            stack = new Stack<Token>();
            labelsStack = new Stack<string>();
        }

        public void Start()
        {
            Table = new List<object>();

            for (int i = 0; i < outputTokenTable.Count; i++)
            {
                Token token = outputTokenTable[i];

                DefineTokenToGeneratePolish(ref token, ref i);

            }

            while (stack.Count > 0) { stack.Pop(); }
        }

        private void DefineTokenToGeneratePolish(ref Token token, ref int i)
        {
            if (token.TokenType == "IDN" || token.TokenType == "CON")
            {
                Polish.Add(token.Name);
                AddToViewTable(token);

                return;
            }

            if (token.Name == "cin" || token.Name == "cout")
            {
                AddToViewTable(token);
                return;
            }

            if (token.Name == "if")
            {
                GenerateConditionPolish(ref token, ref i);
                return;
            }

            if (token.Name == "for")
            {
                GenerateCyclePolish(ref token, ref i);
                return;
            }

            GeneratePolish(ref token);
        }

        private void GenerateCyclePolish(ref Token token, ref int i)
        {
            Polish.Add($"r{cycleDesignationCounter}");
            Polish.Add("1");
            Polish.Add("=");

            stack.Push(token);
            AddToViewTable(token);

            i++;

            bool baseFlg = false;
            string iteratorName = string.Empty;


            while (i < outputTokenTable.Count)
            {
                token = outputTokenTable[i];

                if (baseFlg)
                {
                    DefineTokenToGeneratePolish(ref token, ref i);

                    if (token.Name == "}")
                    {
                        cycleDesignationCounter--;
                        Polish.Add($"r{cycleDesignationCounter}");
                        Polish.Add("0");
                        Polish.Add("=");

                        string labelName = labelsStack.Pop();

                        Polish.Add($"{labelsStack.Pop()} JMP");

                        LabelsTable.Add(labelName, Polish.Count);
                        Polish.Add($"{labelName}:");

                        while (stack.Pop().Name != "for") { }

                        AddToViewTable(token);
                        return;
                    }
                }
                else
                {
                    if (token.TokenType == "IDN" || token.TokenType == "CON")
                    {
                        Polish.Add(token.Name);
                        AddToViewTable(token);

                        i++;
                        continue;
                    }

                    if ("()".Contains(token.Name))
                    {
                        i++;
                        if (token.Name == "(")
                        {
                            iteratorName = outputTokenTable[i].Name;
                        }
                        AddToViewTable(token);
                        continue;
                    }

                    if (stack.Count == 0)
                    {
                        stack.Push(token);
                    }
                    else
                    {
                        while (stack.Count > 0)
                        {
                            if (stack.Peek().TokenType != "label" && stack.Peek().GetPriority() >= token.GetPriority())
                            {
                                if (stack.Peek().Name != "for")
                                {
                                    Token stackHead = stack.Pop();

                                    Polish.Add(stackHead.TokenType.Equals("@") ? stackHead.TokenType : stackHead.Name);

                                    AddToViewTable(token);
                                }
                            }
                            else if (token.Name == ";")
                            {
                                if (Polish.Last() == "=")
                                {
                                    string newLabelName = GetNewLabel();


                                    LabelsTable.Add(newLabelName, Polish.Count);

                                    Polish.Add($"{newLabelName}:");
                                    stack.Push(new Token { Name = newLabelName, TokenType = "label" });

                                    AddToViewTable(token);

                                    break;
                                }
                                else if (Checker.IsRelation(Polish.Last()))
                                {
                                    string newLabelName = GetNewLabel();

                                    Polish.Add($"{newLabelName} JNE");
                                    stack.Push(new Token { Name = newLabelName, TokenType = "label" });

                                    Polish.Add($"r{cycleDesignationCounter}");
                                    Polish.Add($"0");
                                    Polish.Add($"==");

                                    cycleDesignationCounter++;

                                    newLabelName = GetNewLabel();
                                    Polish.Add($"{newLabelName} JNE");
                                    stack.Push(new Token { Name = newLabelName, TokenType = "label" });

                                    Polish.Add(iteratorName);

                                    AddToViewTable(token);
                                    break;
                                }
                            }
                            else if (token.Name == "{" && stack.Peek().Name == "for")
                            {

                                break;
                            }
                            else
                            {
                                if (token.Name == "{" && !baseFlg)
                                {
                                    Polish.Add("=");
                                    string labelName = labelsStack.Pop();

                                    LabelsTable.Add(labelName, Polish.Count);
                                    Polish.Add($"{labelName}:");

                                    AddToViewTable(token);

                                    baseFlg = true;
                                    break;
                                }

                                stack.Push(token);
                                break;
                            }
                        }
                    }
                }
                i++;
            }
        }

        private void GenerateConditionPolish(ref Token token, ref int i)
        {
            bool isShortCondition = false;

            stack.Push(token);
            i++;

            while (i < outputTokenTable.Count)
            {
                token = outputTokenTable[i];

                DefineTokenToGeneratePolish(ref token, ref i);

                if ((token.Name == ")" && Checker.IsRelation(Polish.Last())))
                {
                    if (outputTokenTable[i + 1].Name != "{")
                    {
                        isShortCondition = true;
                    }
                    string newLabelName = GetNewLabel();

                    Polish.Add($"{newLabelName} JNE");
                    stack.Push(new Token { Name = newLabelName, TokenType = "label" });
                    AddToViewTable(token);
                }

                if (token.Name == "}" || (token.Name == ";" && isShortCondition))
                {
                    string labelName = labelsStack.Pop();

                    LabelsTable.Add(labelName, Polish.Count);
                    Polish.Add($"{labelName}:");

                    while (stack.Pop().Name != "if") { }

                    AddToViewTable(token);

                    i++;
                    token = outputTokenTable[i];

                    return;
                }

                i++;
            }
        }


        private void GeneratePolish(ref Token token)
        {
            if (stack.Count == 0 || token.Name == "(")
            {
                stack.Push(token);
                AddToViewTable(token);
            }
            else
            {
                while (stack.Count > 0)
                {
                    if (stack.Peek().Name == "(" && token.Name == ")")
                    {
                        stack.Pop();
                        AddToViewTable(token);
                        break;
                    }

                    if (stack.Peek().TokenType != "label" && stack.Peek().GetPriority() >= token.GetPriority())
                    {

                        if ((token.Name == "{" && stack.Peek().Name == "if") || token.Name == "}")
                            return;

                        Token stackHead = stack.Pop();

                        if (!",;{".Contains(stackHead.Name))
                        {
                            Polish.Add(stackHead.TokenType.Equals("@") ? stackHead.TokenType : stackHead.Name);
                        }

                        AddToViewTable(token);
                    }
                    else
                    {
                        stack.Push(token);
                        AddToViewTable(token);
                        break;
                    }
                }
            }
        }

        private string GetNewLabel()
        {
            string label = $"l{labelCounter}";
            labelsStack.Push(label);
            labelCounter++;

            return label;
        }

        private void AddToViewTable(Token token)
        {
            var outputStack = stack.Select(s => s.Name);

            Table.Add(
                new
                {
                    InputString = token.Name,
                    Stack = string.Join(" ", outputStack),
                    Polis = string.Join(" ", Polish)
                }
            );
        }
    }
}
