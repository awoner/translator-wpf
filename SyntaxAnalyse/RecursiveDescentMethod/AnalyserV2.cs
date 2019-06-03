using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using Translator_desktop.LexicalAnalyse;
using Translator_desktop.LexicalAnalyse.Tables;
using Translator_desktop.SyntaxAnalyse.OperatorPrecedenceMethod;

namespace Translator_desktop.SyntaxAnalyse.RecursiveDescentMethod
{
    public class AnalyserV2
    {

        private static IList<string> errors;
        private static IList<Token> outputTokens;
        private Dictionary<string, Action> map = new Dictionary<string, Action>();
        public List<Rule> Grammar = new List<Rule>(); 

        public AnalyserV2()
        {
            errors = new List<string>();
            outputTokens = OutputTokenTable.Table;
            try
            {
                Grammar = new List<Rule>();

                string projectRoot = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                string path = Directory.GetFiles(projectRoot, "Grammar.json", SearchOption.AllDirectories).FirstOrDefault();

                using (StreamReader file = File.OpenText(path))
                {
                    ParseGrammar(file);
                    Parse();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ParseGrammar(StreamReader sr)
        {
            //parsing rules from json
            List<RuleBuffer> rules = (List<RuleBuffer>)new JsonSerializer().Deserialize(sr, typeof(List<RuleBuffer>));

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

        public void Parse()
        {
            foreach (var rule in Grammar)
            {
                string methodName = rule.LeftPart.Name;

                map[methodName]();

            }
        }

        //public bool Prog()
        //{

        //} 
    }
}
