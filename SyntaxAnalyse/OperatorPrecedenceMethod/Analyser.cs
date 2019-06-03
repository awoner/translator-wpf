using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Translator_desktop.LexicalAnalyse;
using Translator_desktop.LexicalAnalyse.Tables;

namespace Translator_desktop.SyntaxAnalyse.OperatorPrecedenceMethod
{
    public class Analyser
    {
        private List<Token> outputTokenTable;
        private List<RelationshipToken> relationshipsTable;
        private Stack<Token> stack;
        public static List<ViewToken> Table { get; private set; }
        public List<Token> PolisOutput { get; set; }
        public string Error { get; set; }

        public class ViewToken
        {
            public int step;
            public static int Step { get; set; } = 0;
            public string Stack { get; set; }
            public string Relation { get; set; }
            public string InputString { get; set; }
            public string Polis { get; set; }

            public ViewToken()
            {
                step = Step++;
            }
        }

        public Analyser()
        {
            outputTokenTable = OutputTokenTable.Table.ToList();
            RelationshipsTable.InitTable();
            relationshipsTable = RelationshipsTable.Table;
            //ViewToken.Step = 0;
            Table = new List<ViewToken>();
            PolisOutput = new List<Token>();
            stack = new Stack<Token>();
        }

        public void Parse()
        {
            while (outputTokenTable.Count > 0)
            {
                Token inputToken = outputTokenTable.First();

                if (stack.Count < 1)
                {
                    stack.Push(inputToken);
                    outputTokenTable.Remove(inputToken);
                    AddTokenToViewTable("<");

                    continue;
                }

                Token stackToken = stack.Peek();

                string relation = relationshipsTable
                    .FirstOrDefault(rl =>
                    {
                        return rl.FirstLinguisticUnit.Name.Equals(GetRelationName(stackToken))
                            && rl.SecondLinguisticUnit.Name.Equals(GetRelationName(inputToken));
                    })?.Relationship;

                AddTokenToViewTable(relation);

                if (relation is null)
                {
                    throw new Exception($"Syntax error on {inputToken.Row} line!");
                }

                if (relation.Equals("<") || relation.Equals("="))
                {
                    stack.Push(inputToken);
                    outputTokenTable.Remove(inputToken);
                }
                else if (stackToken.Name.Equals(RelationshipsTable.Grammar.First().LeftPart.Name) ||
                   stack.Count != 0 && stack.Peek().Name.Equals("<E>") && inputToken.Name.Equals("$"))
                {
                    stack.Push(inputToken);
                    return;
                }
                else if (relation.Equals(">"))
                {
                    SetPossibleBasis();
                }
            }
        }

        public double GetPolisResult()
        {
            if (PolisOutput.Count != 0)
            {
                double first = 0, second = 0;
                Stack<double> polisNums = new Stack<double>();

                for (int i = 0; i < PolisOutput.Count; i++)
                {
                    if (Checker.IsOperator(PolisOutput[i].Name))
                    {
                        switch (PolisOutput[i].Name)
                        {
                            case "+":
                                second = polisNums.Pop();
                                first = polisNums.Pop();
                                polisNums.Push(first + second);
                                break;

                            case "-":
                                second = polisNums.Pop();
                                first = polisNums.Pop();
                                polisNums.Push(first - second);
                                break;

                            case "/":
                                second = polisNums.Pop();
                                first = polisNums.Pop();

                                if (second == 0.0)
                                {
                                    Error = "Division by 0.";
                                    return 0;
                                }

                                polisNums.Push(first / second);
                                break;

                            case "*":
                                second = polisNums.Pop();
                                first = polisNums.Pop();
                                polisNums.Push(first * second);
                                break;

                            case "@":
                                polisNums.Push(-polisNums.Pop());
                                break;

                            default:
                                throw new InvalidOperationException();
                        }
                    }
                    else
                    {
                        if (PolisOutput[i].TokenType != "IDN")
                            double.TryParse(PolisOutput[i].Name, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out first);
                        else
                            first = (double)PolisOutput[i].Value;//double.TryParse(, System.Globalization.NumberStyles.Any, CultureInfo.InvariantCulture, out first);
                        polisNums.Push(first);
                    }
                }
                return polisNums.Pop();
            }

            throw new NotImplementedException();
        }

        private string GetRelationName(Token token)
        {
            return (string.IsNullOrEmpty(token.TokenType)) ? token.Name : token.TokenType;
        }    

        private void SetPossibleBasis()
        {
            string relation = string.Empty;
            List<Token> possibleBasis = new List<Token>();

            while (stack.Count != 0)
            {
                Token currentToken = stack.Pop();
                Token previousToken = null;

                if (stack.Count != 0 && !stack.Peek().Name.Equals("$"))
                {
                    previousToken = stack.Peek();

                    relation = relationshipsTable.FirstOrDefault(rl =>
                    {
                        return rl.FirstLinguisticUnit.Name.Equals(GetRelationName(previousToken))
                           && rl.SecondLinguisticUnit.Name.Equals(GetRelationName(currentToken));
                    })?.Relationship;
                }
                else
                {
                    relation = "<";
                }

                if (relation.Equals("<"))
                {
                    possibleBasis.Add(currentToken);

                    if (TryFindBaseElement(possibleBasis, out Rule basis, previousToken))
                    {
                        var basisName = basis.LeftPart.Name;

                        stack.Push(new Token { Name = basisName });
                        return;
                    }
                    else
                    {
                        throw new Exception($"Relationship is not defined {((previousToken?.Row is null) ? currentToken.Row : previousToken.Row)} line.");
                    }                    
                }
                else if (relation.Equals(string.Empty))
                {
                    relation = "<";
                    stack.Push(currentToken);
                }
                else
                {
                    possibleBasis.Add(currentToken);
                }
            }
        }

        private bool TryFindBaseElement(List<Token> possibleBasis, out Rule basis, Token previousToken)
        {
            bool ListCompare(List<Token> tokens, List<LinguisticUnit> linguisticUnits)
            {
                if (tokens.Count != linguisticUnits.Count) return false;

                for (int i = 0; i < tokens.Count; i++)
                {
                    if (!GetRelationName(tokens[i]).Equals(linguisticUnits[i].Name)) return false;
                }

                return true;
            }

            possibleBasis.Reverse();

            var basises = RelationshipsTable.Grammar.Where(r => r.RightParts.Select(rp => rp.LinguisticUnits).Any(lu => ListCompare(possibleBasis, lu)));

            if (basises != null)
            {
                if (string.Join(" ", possibleBasis.Select(pb => pb.Name)).Equals("- <T1>"))
                {
                    var terminal = (Token)possibleBasis.First().Clone();
                    terminal.Name = "@";
                    PolisOutput.Add(terminal);
                }
                else
                {
                    var terminals = possibleBasis.Where(pb => !(new Regex(@"^<.+>$").IsMatch(pb.Name)) && !"()".Contains(pb.Name));
                    PolisOutput.AddRange(terminals);
                }
                
            }

            //TODO: rewrite next code



            //if (previousToken != null)
            //{

            //}
            //else
            //{
            //    if(possibleBasis.First().TokenType.)
            //}

            if (previousToken != null)
            {
                //basis = basises.FirstOrDefault();
                var cond = (stack.Count != 0) ? stack.Pop() : null;
                if (previousToken.Name.Equals("(") || previousToken.Name.Equals("#") || possibleBasis.First().TokenType.Equals("IDN")/*|| "/*".Contains(previousToken.Name)*/
                                                                                                                                    /*|| cond != null && (new Regex(@"^<.+>$").IsMatch(stack.Peek().Name)) && (previousToken.Equals("-") || !possibleBasis[0].Name.Equals("<F>"))*/)
                {
                    basis = basises.LastOrDefault();
                }
                else
                {
                    basis = basises.FirstOrDefault();
                }

                if (previousToken.Name.Equals("(") && basis.LeftPart.Name.Equals("<F1>"))
                {
                    basis = basises.FirstOrDefault();
                }
                if (cond != null)
                {
                    stack.Push(cond);

                }
            }
            else
            {
                basis = basises.FirstOrDefault();
            }

            return (basis is null) ? false : true;
        }

        private void AddTokenToViewTable(string relation = null)
        {
            Table.Add(new ViewToken
            {
                InputString = string.Join(" ", outputTokenTable.Select(ot => ot.Name)),
                Relation = relation,
                Stack = string.Join(" ", stack.Reverse().Select(st => st.Name)),
                Polis = string.Join(" ", PolisOutput.Select(st => st.Name))
            });
        }
    }
}
