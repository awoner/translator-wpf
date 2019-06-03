using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;

namespace Translator_desktop.SyntaxAnalyse.OperatorPrecedenceMethod
{
    public static class RelationshipsTable
    {
        public static ICollection<Rule> Grammar { get; set; }
        public static List<RelationshipToken> RelationshipsTableBuffer { get; private set; }
        public static List<RelationshipToken> Table { get; private set; }
        public static List<RuleBuffer> SimpleGrammar { get; set; }

        public static void InitTable()
        {
            try
            {
                Grammar = new List<Rule>();
                Table = new List<RelationshipToken>();

                string projectRoot = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                string path = Directory.GetFiles(projectRoot, "StratifiedGrammar.json", SearchOption.AllDirectories).FirstOrDefault();

                using (StreamReader file = File.OpenText(path))
                {
                    ParseGrammar(file);
                    BuildTable();
                    RelationshipsTableBuffer = Table;
                    SaveToFile();
                    GetConflicts();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static void SaveToFile()
        {
            string str = string.Empty;
            foreach (var item in Table)
            {
                str += item.FirstLinguisticUnit.Name + " [" + item.Relationship + "] " + item.SecondLinguisticUnit.Name + "\n";
            }

            File.WriteAllText($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\relationTable.txt", str);
        }

        private static void ParseGrammar(StreamReader sr)
        {
            //parsing rules from json
            List<RuleBuffer> rules = (List<RuleBuffer>)new JsonSerializer().Deserialize(sr, typeof(List<RuleBuffer>));

            SimpleGrammar = rules;

            foreach (RuleBuffer rule in rules)
            {
                var newRule = new Rule
                {
                    //set left part of json like a non terminal
                    LeftPart = new LinguisticUnit { Name = rule.LeftPart.Trim(), Type = LinguisticUnitType.NonTerminal }
                };

                //get all right parts
                string[] rightParts = rule.RightPart.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string rightPart in rightParts)
                {
                    var newRightPart = new RightPart();

                    //get all linguistic units from right part of rule
                    string[] linguisticUnits = rightPart.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string linguisticUnit in linguisticUnits)
                    {
                        newRightPart.LinguisticUnits.Add(new LinguisticUnit
                        {
                            Name = linguisticUnit,

                            //define linguistic unit as terminal or non terminal by reg. exp.
                            Type = new Regex(@"^<.+>$").IsMatch(linguisticUnit) ? LinguisticUnitType.NonTerminal : LinguisticUnitType.Terminal
                        });
                    }
                    newRule.RightParts.Add(newRightPart);
                }
                Grammar.Add(newRule);
            }
        }

        public static List<LinguisticUnit> GetFirst(LinguisticUnit nonTerminal)
        {
            var first = new List<LinguisticUnit>();
            var rule = Grammar.First(r => r.LeftPart.Name == nonTerminal.Name);

            foreach (var rightPart in rule.RightParts)
            {
                first.Add(rightPart.LinguisticUnits.First());
            }

            return first;
        }

        public static List<LinguisticUnit> GetFirstPlus(LinguisticUnit nonTerminal, List<LinguisticUnit> firstPlus = null)
        {
            firstPlus = firstPlus ?? new List<LinguisticUnit>();
            var rule = Grammar.First(r => r.LeftPart.Name == nonTerminal.Name);

            foreach (var rightPart in rule.RightParts)
            {
                if (rightPart.LinguisticUnits.First().Type == LinguisticUnitType.NonTerminal
                    && !firstPlus.Any(f => f.Name == rightPart.LinguisticUnits.First().Name))
                {
                    firstPlus.Add(rightPart.LinguisticUnits.First());
                    GetFirstPlus(rightPart.LinguisticUnits.First(), firstPlus);
                }
                else
                {
                    if (!firstPlus.Any(f => f.Name == rightPart.LinguisticUnits.First().Name))
                    {
                        firstPlus.Add(rightPart.LinguisticUnits.First());
                    }
                }
            }
               
            return firstPlus;
        }

        public static List<LinguisticUnit> GetLast(LinguisticUnit nonTerminal)
        {
            var last = new List<LinguisticUnit>();
            var rule = Grammar.First(r => r.LeftPart.Name == nonTerminal.Name);

            foreach (var rightPart in rule.RightParts)
            {
                last.Add(rightPart.LinguisticUnits.Last());
            }

            return last;
        }

        public static List<LinguisticUnit> GetLastPlus(LinguisticUnit nonTerminal, List<LinguisticUnit> lastPlus = null)
        {
            lastPlus = lastPlus ?? new List<LinguisticUnit>();
            var rule = Grammar.First(r => r.LeftPart.Name == nonTerminal.Name);

            foreach (var rightPart in rule.RightParts)
            {
                if (rightPart.LinguisticUnits.Last().Type == LinguisticUnitType.NonTerminal
                     && !lastPlus.Any(l => l.Name == rightPart.LinguisticUnits.Last().Name))
                {
                    lastPlus.Add(rightPart.LinguisticUnits.Last());
                    GetLastPlus(rightPart.LinguisticUnits.Last(), lastPlus);
                }
                else
                {
                    if (!lastPlus.Any(l => l.Name == rightPart.LinguisticUnits.Last().Name))
                    {
                        lastPlus.Add(rightPart.LinguisticUnits.Last());
                    }
                }
            }

            return lastPlus;
        }

        public static void BuildTable()
        {
            LinguisticUnit dollar = new LinguisticUnit { Name = "$" };

            void AddToRelationshipsTable(RelationshipToken relationshipToken)
            {
                if (Table.FirstOrDefault(rt => rt.FirstLinguisticUnit.Name.Equals(relationshipToken.FirstLinguisticUnit.Name)
                                             && rt.Relationship.Equals(relationshipToken.Relationship)
                                             && rt.SecondLinguisticUnit.Name.Equals(relationshipToken.SecondLinguisticUnit.Name)) is null)
                {
                    Table.Add(relationshipToken);
                }
            }

            foreach (var rule in Grammar)
            {
                foreach (var rightPart in rule.RightParts)
                {
                    for (int i = 0; i < rightPart.LinguisticUnits.Count; i++)
                    {
                        if (i + 1 < rightPart.LinguisticUnits.Count)
                        {
                            var equalRelation = new RelationshipToken
                            {
                                FirstLinguisticUnit = rightPart.LinguisticUnits[i],
                                SecondLinguisticUnit = rightPart.LinguisticUnits[i + 1],
                                Relationship = "="
                            };

                            AddToRelationshipsTable(equalRelation);
                            
                            if (equalRelation.SecondLinguisticUnit.Type == LinguisticUnitType.NonTerminal)
                            {
                                var firstPlus = GetFirstPlus(equalRelation.SecondLinguisticUnit);

                                foreach (var first in firstPlus)
                                {
                                    var lowerRelation = new RelationshipToken
                                    {
                                        FirstLinguisticUnit = equalRelation.FirstLinguisticUnit,
                                        SecondLinguisticUnit = first,
                                        Relationship = "<"
                                    };

                                    AddToRelationshipsTable(lowerRelation);
                                }
                            }

                            if (equalRelation.FirstLinguisticUnit.Type == LinguisticUnitType.NonTerminal)
                            {
                                var lastPlus = GetLastPlus(equalRelation.FirstLinguisticUnit);

                                foreach (var last in lastPlus)
                                {
                                    var higherRelation = new RelationshipToken
                                    {
                                        FirstLinguisticUnit = last,
                                        SecondLinguisticUnit = equalRelation.SecondLinguisticUnit,
                                        Relationship = ">"
                                    };

                                    AddToRelationshipsTable(higherRelation);
                                }
                            }

                            if (equalRelation.FirstLinguisticUnit.Type == LinguisticUnitType.NonTerminal
                              && equalRelation.SecondLinguisticUnit.Type == LinguisticUnitType.NonTerminal)
                            {
                                var lastPlus = GetLastPlus(equalRelation.FirstLinguisticUnit);
                                var firstPlus = GetFirstPlus(equalRelation.SecondLinguisticUnit);

                                foreach (var last in lastPlus)
                                {
                                    foreach (var first in firstPlus)
                                    {
                                        AddToRelationshipsTable(new RelationshipToken
                                        {
                                            FirstLinguisticUnit = last,
                                            SecondLinguisticUnit = first,
                                            Relationship = ">"
                                        });
                                    }
                                }
                            }
                        }

                        AddToRelationshipsTable(new RelationshipToken
                        {
                            FirstLinguisticUnit = rightPart.LinguisticUnits[i],
                            SecondLinguisticUnit = dollar,
                            Relationship = ">"
                        });

                        AddToRelationshipsTable(new RelationshipToken
                        {
                            FirstLinguisticUnit = dollar,
                            SecondLinguisticUnit = rightPart.LinguisticUnits[i],
                            Relationship = "<"
                        });
                    } 
                }
            }

            AddToRelationshipsTable(new RelationshipToken
            {
                FirstLinguisticUnit = dollar,
                SecondLinguisticUnit = Grammar.First().LeftPart,
                Relationship = "<"
            });

            AddToRelationshipsTable(new RelationshipToken
            {
                FirstLinguisticUnit = Grammar.First().LeftPart,
                SecondLinguisticUnit = dollar,
                Relationship = ">"
            });
        }

        public static List<RelationshipToken> GetConflicts()
        {
            Dictionary<RelationshipToken, RelationshipToken> conflicts = new Dictionary<RelationshipToken, RelationshipToken>();
            foreach (var token in Table)
            {
                RelationshipToken conflict = Table.FirstOrDefault(c => c.FirstLinguisticUnit.Name == token.FirstLinguisticUnit.Name
                            && c.SecondLinguisticUnit.Name == token.SecondLinguisticUnit.Name
                            && c.Relationship != token.Relationship);
                if (conflict != null 
                    && !conflicts.Any(c => 
                        c.Key.FirstLinguisticUnit.Name == conflict.FirstLinguisticUnit.Name
                        && c.Key.SecondLinguisticUnit.Name == conflict.SecondLinguisticUnit.Name
                        && c.Key.Relationship == conflict.Relationship)
                    && !conflicts.Any(c =>
                        c.Key.FirstLinguisticUnit.Name == token.FirstLinguisticUnit.Name
                        && c.Key.SecondLinguisticUnit.Name == token.SecondLinguisticUnit.Name
                        && c.Key.Relationship == token.Relationship))
                {
                    conflicts.Add(token, conflict);
                }
            }

            string str = string.Empty;
            foreach (var conflict in conflicts)
            {
                str += $"|\t{conflict.Key.FirstLinguisticUnit.Name}\t[ {conflict.Key.Relationship} ]\t{conflict.Key.SecondLinguisticUnit.Name}\t|";
                str += $"\t{conflict.Value.FirstLinguisticUnit.Name}\t[ {conflict.Value.Relationship} ]\t{conflict.Value.SecondLinguisticUnit.Name}\t|{Environment.NewLine}";
            }
            File.WriteAllText($@"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\conflicts.txt", str);
            return new List<RelationshipToken>();
        }
    }
}
