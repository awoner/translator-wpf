using System;
using System.IO;
using System.Linq;
using System.Windows;
using Translator_desktop.LexicalAnalyzer.Tables;

namespace Translator_desktop.LexicalAnalyzer
{
    public class Analyzer
    {
        public Analyzer()
        {
            TokenTable.InitTable();
            ConTable.InitTable();
            IdnTable.InitTable();
            OutputTokenTable.InitTable();
        }

        public void Parse(string[] programCode)
        {
            try
            {
                string token = "";
                for (int i = 0; i < programCode.Length; i++)
                {
                    int j = 0;
                    foreach (char ch in programCode[i])
                    {
                        if (Checker.IsConstant(ch) || Checker.IsLetter(ch) || (Checker.IsSign(ch) && token != "" && Checker.IsExponent(token.Last())))
                        {
                            token += ch;
                        }
                        else if ((Checker.IsSingleCharacterSeparator(ch) || Checker.IsWhiteSeparator(ch) || Checker.IsPossibleDoubleSeparator(ch)))
                        {
                            if (TokenTable.Contains(token) && Checker.IsType(token))
                            {
                                Checker.Type = token;
                            }
                            else if (Checker.IsDigit(token))
                            {
                                string penultimateToken = OutputTokenTable.GetTable()[OutputTokenTable.GetTable().Count - 2].Name;

                                if (OutputTokenTable.GetTable().Last().Name == "=" && IdnTable.Contains(penultimateToken))
                                {
                                    if (Checker.GetType(token) == IdnTable.GetType(penultimateToken))
                                    {
                                        ConTable.Add(token, IdnTable.GetType(penultimateToken));
                                    }
                                    else
                                    {
                                        throw new Exception("Error on " + (i + 1) + " line!\tInvalid type of constant. Unable to cast '" +
                                                            Checker.GetType(token) + "' to '" + IdnTable.GetType(penultimateToken) + "'");
                                    }
                                }
                                else if (!ConTable.Contains(token))
                                {
                                    ConTable.Add(token);
                                }
                            }
                            else if (token != "")
                            {
                                if (OutputTokenTable.GetTable().Count != 0 && Checker.IsType(OutputTokenTable.GetTable().Last().Name))
                                {
                                    if (IdnTable.Contains(token))
                                    {
                                        throw new Exception("Error on " + (i + 1) + " line!\tThe variable '" + token + "' is already declared.");
                                    }
                                    else
                                    {
                                        IdnTable.Add(token);
                                    }
                                }
                                else if (OutputTokenTable.GetTable().Count != 0 && OutputTokenTable.GetTable().Last().Name == ",")
                                {
                                    if (!IdnTable.Contains(token))
                                    {
                                        IdnTable.Add(token);
                                    }
                                    else
                                    {
                                        throw new Exception("Error on " + (i + 1) + " line!\tThe variable '" + token + "' is already declared.");
                                    }
                                }
                                else if (!IdnTable.Contains(token) && !TokenTable.Contains(token))
                                {
                                    throw new Exception("Error on " + (i + 1) + " line!\tThe variable '" + token + "' wasn't declared.");
                                }
                            }

                            //TO DO: Unable to cast check
                            if (!Checker.IsAbleToCast(token))
                            {
                                throw new Exception(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>Error on " + (i + 1) + " line!\tInvalid type of constant. Unable to cast '" +
                                                        IdnTable.GetType(token) + "' to '" + IdnTable.GetType(OutputTokenTable.GetTable()[OutputTokenTable.GetTable().Count - 2].Name) + "'");
                            }

                            if (ConTable.Contains(token) || IdnTable.Contains(token) || (TokenTable.Contains(token) && !Checker.IsDoubleSeparator(token + ch)))
                            {
                                OutputTokenTable.Add(i + 1, token);
                                token = "";
                            }

                            if (Checker.IsPossibleDoubleSeparator(ch)/* || buffer != ""*/)
                            {
                                if (!Checker.IsDoubleSeparator(token) && Checker.IsDoubleSeparator(ch.ToString() + (programCode[i].Length - 1 != j ? programCode[i][j + 1].ToString() : "")))
                                {
                                    token += ch;
                                    j++;
                                    continue;
                                }
                                else
                                {
                                    OutputTokenTable.Add(i + 1, token + ch);
                                    token = "";
                                }
                            }
                            else if (Checker.IsSingleCharacterSeparator(ch))
                            {
                                OutputTokenTable.Add(i + 1, token + ch);
                            }
                        }
                        else
                        {
                            throw new Exception("Error on " + (i + 1) + " line!\tUndefined exeption.");
                        }
                        j++;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Alert", MessageBoxButton.OK);
            }
        }

    }
}