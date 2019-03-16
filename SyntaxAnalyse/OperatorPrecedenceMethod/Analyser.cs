using System;
using System.Collections.Generic;
using System.Linq;
using Translator_desktop.LexicalAnalyse.Tables;

namespace Translator_desktop.SyntaxAnalyse.OperatorPrecedenceMethod
{
    public class Analyser
    {
        public class ViewToken
        {
            public int step;
            public static int Step { get; set; } = 0;
            public string Stack { get; set; }
            public string Relation { get; set; }
            public string InputString { get; set; }

            public ViewToken()
            {
                step = Step++;
            }
        }

        private static IList<ViewToken> viewTokenTable;
        private IList<Token> outputTokenTable;
        public IList<RelationshipToken> relationshipsTable;
        private Stack<Token> stack;
        public static IList<ViewToken> Table { get => viewTokenTable; }

        public Analyser()
        {
            RelationshipsTable.InitTable();
            relationshipsTable = RelationshipsTable.Table;
            outputTokenTable = OutputTokenTable.Table.ToList();
            ViewToken.Step = 0;
            viewTokenTable = new List<ViewToken>();
            stack = new Stack<Token>();
        }

        public void Parse()
        {
            Token TrySetGeneralizeName(Token token)
            {
                if (!TokenTable.Contains(token.Name))
                {
                    if (IdnTable.Contains(token.Name))
                    {
                        return new Token { Code = token.Code, Name = "id", Row = token.Row };
                    }
                    else if (ConTable.Contains(token.Name))
                    {
                        return new Token { Code = token.Code, Name = "con", Row = token.Row };
                    }
                    else
                    {
                        throw new Exception("Unknow token.");
                    }
                }

                return token;
            }

            while (outputTokenTable.Count > 0)
            {                
                Token token = outputTokenTable.First();

                if (stack.Count < 2)
                {
                    stack.Push(token);
                    outputTokenTable.Remove(token);

                    if (stack.Count == 1)
                    {
                        AddTokenToViewTable("<");
                    }
                    
                    continue;
                }

                Token stackToken = stack.Peek();
                Token inputToken = TrySetGeneralizeName(token);

                string relation = relationshipsTable
                    .FirstOrDefault(rl => 
                    {
                        return rl.FirstLinguisticUnit.Name.Equals(stackToken.Name) 
                            && rl.SecondLinguisticUnit.Name.Equals(inputToken.Name);
                    })?.Relationship;

                AddTokenToViewTable(relation);

                if (stackToken.Name.Equals(RelationshipsTable.Grammar.First().LeftPart.Name))
                {
                    stack.Push(inputToken);
                    return;
                }
                else if (relation.Equals("<") || relation.Equals("="))
                {
                    stack.Push(inputToken);
                    outputTokenTable.Remove(token);
                }
                else if (relation.Equals(">"))
                {
                    SetPossibleBasis();
                }
                else if (relation is null)
                {
                    throw new Exception($"Syntax error on {inputToken.Row} line!");
                }
            }
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
                        return rl.FirstLinguisticUnit.Name.Equals(previousToken.Name)
                           && rl.SecondLinguisticUnit.Name.Equals(currentToken.Name);
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
                        throw new Exception($"Relationship is not defined {previousToken.Row} line.");
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
                    if (!tokens[i].Name.Equals(linguisticUnits[i].Name)) return false;
                }

                return true;
            }

            possibleBasis.Reverse();

            var basises = RelationshipsTable.Grammar.Where(r => r.RightParts.Select(rp => rp.LinguisticUnits).Any(lu => ListCompare(possibleBasis, lu)));
            basis = (!(previousToken is null) && (previousToken.Name.Equals("(") || previousToken.Name.Equals("#"))) ? basises.Last() : basises.First();

            return (basis is null) ? false : true;
        }

        private void AddTokenToViewTable(string relation = null)
        {
            viewTokenTable.Add(new ViewToken
            {
                InputString = string.Join(" ", outputTokenTable.Select(ot => ot.Name)),
                Relation = relation,
                Stack = string.Join(" ", stack.Reverse().Select(st => st.Name))
            });
        }
    }
}
