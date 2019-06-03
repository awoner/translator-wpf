using System;
using Translator_desktop.LexicalAnalyse.Tables;
using System.Collections.Generic;
using System.Windows;
using Translator_desktop.LexicalAnalyse;

namespace Translator_desktop.SyntaxAnalyse.RecursiveDescentMethod
{
    public class Analyser
    {
        private static IList<string> errors;
        private static IList<Token> outputTokens;

        public Analyser()
        {
            errors = new List<string>();
            outputTokens = OutputTokenTable.Table;
        }

        public IList<string> GetErrors()
        {
            return errors;
        }

        public bool Increment(ref int i)
        {
            if (i < outputTokens.Count - 1)
            {
                i++;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool Parse()
        {
            if (outputTokens.Count == 0)
            {
                return false;
            }
            int i = 0;

            try
            {
                return Programm(ref i);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Alert", MessageBoxButton.OK);
            }
            return false;
        }

        private bool Programm(ref int i)
        {
            if (DeclarationsList(ref i))
            {
                if (outputTokens[i].Name == "{")
                {
                    if (!Increment(ref i)) { return false; }

                    if (OperatorsList(ref i))
                    {
                        if (outputTokens[i].Name == "}")
                        {
                            if (i < outputTokens.Count - 1)
                            {
                                i++;
                                for (int j = i; j < outputTokens.Count; j++)
                                {
                                    errors.Add(" > Error on " + outputTokens[j].Row + " line!\tEnexpected symbol '" + outputTokens[j].Name + "'!");
                                }
                                return false;
                            }
                            return true;
                        }
                        else
                        {
                            errors.Add(" > Error on " + outputTokens[i].Row + " line!\tNo closing bracket '}'!");
                            return false;
                        }
                    }
                    else
                    {
                        errors.Add(" > Error on " + outputTokens[i].Row + " line!\tUnxpected end of file!");
                        return false;
                    }
                }
                else
                {
                    errors.Add(" > Error on " + outputTokens[i].Row + " line!\tNo open bracket '{'!");
                    return false;
                }
            }
            else
            {
                errors.Add(" > Error on " + outputTokens[i].Row + " line!\tExpected variable list.");
                return false;
            }
        }

        private bool OperatorsList(ref int i)
        {
            if (Operator(ref i))
            {
                if (outputTokens[i].Name == ";")
                {
                     if (!Increment(ref i)) { return false; }

                    while (outputTokens[i].Name != "}")
                    {
                        if (Operator(ref i))
                        {
                            if (outputTokens[i].Name != ";")
                            {
                                errors.Add(" > Error on " + outputTokens[i].Row + " line!\tExpected ';' after operator");
                                return false;
                            }
                            if (!Increment(ref i)) { return false; }
                        }
                        else
                        {
                            errors.Add(" > Error on " + outputTokens[i].Row + " line!\tExpected operator!");
                            return false;
                        }
                    }

                    return true;
                }
                else
                {
                    errors.Add(" > Error on " + outputTokens[i].Row + " line!\tMissed ';' after first operator!");
                    return false;
                }
            }
            else
            {
                errors.Add(" > Error on " + outputTokens[i].Row + " line!\tMissed first operator!");
                return false;
            }
        }

        private bool Operator(ref int i)
        {
            if (Cycle(ref i))
            {
                return true;
            }
            if (Condition(ref i))
            {
                return true;
            }
            if (Assignment(ref i))
            {
                return true;
            }
            if (Input(ref i))
            {
                return true;
            }
            if (Output(ref i))
            {
                return true;
            }

            return false;
        }

        private bool Input(ref int i)
        {
            if (outputTokens[i].Name == "cin")
            {
                if (!Increment(ref i)) { return false; }

                if (outputTokens[i].Name == "<<")
                {
                    if (!Increment(ref i)) { return false; }

                    if (IdnTable.Contains(outputTokens[i].Name))
                    {
                        if (!Increment(ref i)) { return false; }

                        while ((outputTokens[i].Name == "<<" && outputTokens[i - 1].Name != "<<") || IdnTable.Contains(outputTokens[i].Name))
                        {
                            if (!Increment(ref i)) { return false; }
                        }

                        return true;
                    }
                    else
                    {
                        errors.Add(" > Error on " + outputTokens[i].Row + " line!\tExpected variable!");
                    }
                }
                else
                {
                    errors.Add(" > Error on " + outputTokens[i].Row + " line!\tExpected '<<'!");
                }

            }
            return false;
        }

        private bool Output(ref int i)
        {
            if (outputTokens[i].Name == "cout")
            {
                if (!Increment(ref i)) { return false; }

                if (outputTokens[i].Name == ">>")
                {
                    if (!Increment(ref i)) { return false; }

                    if (IdnTable.Contains(outputTokens[i].Name))
                    {
                        if (!Increment(ref i)) { return false; }

                        while ((outputTokens[i].Name == ">>" && outputTokens[i - 1].Name != ">>") || IdnTable.Contains(outputTokens[i].Name))
                        {
                            if (!Increment(ref i)) { return false; }
                        }

                        return true;
                    }
                    else
                    {
                        errors.Add(" > Error on " + outputTokens[i].Row + " line!\tExpected variable!");
                    }
                }
                else
                {
                    errors.Add(" > Error on " + outputTokens[i].Row + " line!\tExpected '>>'!");
                }
            }
            return false;
        }

        private bool Assignment(ref int i)
        {
            if (IdnTable.Contains(outputTokens[i].Name))
            {
                if (!Increment(ref i)) { return false; }

                if (outputTokens[i].Name == "=")
                {
                    if (!Increment(ref i)) { return false; }

                    if (Expression(ref i))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool Condition(ref int i)
        {
            if (outputTokens[i].Name == "if")
            {
                if (!Increment(ref i)) { return false; }

                if (outputTokens[i].Name == "(")
                {
                    if (!Increment(ref i)) { return false; }

                    if (Relation(ref i))
                    {
                        if (outputTokens[i].Name == ")")
                        {
                            if (!Increment(ref i)) { return false; }

                            if (OperatorBlock(ref i))
                            {
                                return true;
                            }
                        }
                        else
                        {
                            errors.Add(" > Error on " + outputTokens[i].Row + " line!\tExpected ')'!");
                        }
                    }
                }
                else
                {
                    errors.Add(" > Error on " + outputTokens[i].Row + " line!\tExpected '('!");
                }
            }

            return false;
        }

        private bool Cycle(ref int i)
        {
            if (outputTokens[i].Name == "for")
            {
                if (!Increment(ref i)) { return false; }

                if (outputTokens[i].Name == "(")
                {
                    if (!Increment(ref i)) { return false; }

                    if (IdnTable.Contains(outputTokens[i].Name))
                    {
                        if (!Increment(ref i)) { return false; }

                        if (outputTokens[i].Name == "=")
                        {
                            if (!Increment(ref i)) { return false; }

                            if (Expression(ref i))
                            {
                                if (outputTokens[i].Name == ";")
                                {
                                    if (!Increment(ref i)) { return false; }

                                    if (Relation(ref i))
                                    {
                                        if (outputTokens[i].Name == ";")
                                        {
                                            if (!Increment(ref i)) { return false; }

                                            if (Expression(ref i))
                                            {
                                                if (outputTokens[i].Name == ")")
                                                {
                                                    if (!Increment(ref i)) { return false; }

                                                    if (OperatorBlock(ref i))
                                                    {
                                                        return true;
                                                    }

                                                    return false;
                                                }
                                                else
                                                {
                                                    errors.Add(" > Error on " + outputTokens[i].Row + " line!\tExpected ')'!");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            errors.Add(" > Error on " + outputTokens[i].Row + " line!\tExpected ';'!");
                                        }
                                    }
                                }
                                else
                                {
                                    errors.Add(" > Error on " + outputTokens[i].Row + " line!\tExpected ';'!");
                                }
                            }
                        }
                        else
                        {
                            errors.Add(" > Error on " + outputTokens[i].Row + " line!\tExpected '='!");
                        }
                    }
                    else
                    {
                        errors.Add(" > Error on " + outputTokens[i].Row + " line!\tnNot initiated variable!");
                    }
                }
                else
                {
                    errors.Add(" > Error on " + outputTokens[i].Row + " line!\tExpected '('!");
                }
            }

            return false;
        }

        private bool OperatorBlock(ref int i)
        {
            if (outputTokens[i].Name == "{")
            {
                if (!Increment(ref i)) { return false; }

                if (OperatorsList(ref i))
                {
                    if (outputTokens[i].Name == "}")
                    {
                        if (!Increment(ref i)) { return false; }

                        return true;
                    }
                    else
                    {
                        errors.Add(" > Error on " + outputTokens[i].Row + " line!\tExpected '}' or operator!");
                    }
                }
            }
            if (Operator(ref i))
            {
                return true;
            }

            errors.Add(" > Error on " + outputTokens[i].Row + " line!\tExpected '{' or operator!");
            return false;
        }

        private bool Relation(ref int i)
        {
            if (outputTokens[i].Name == "true" || outputTokens[i].Name == "false")
            {
                if (!Increment(ref i)) { return false; }

                return true;
            }
            if (Expression(ref i))
            {
                if (Sign(ref i))
                {
                    if (Expression(ref i))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool Sign(ref int i)
        {
            if (Checker.IsDoubleSeparator(outputTokens[i].Name) || Checker.IsSingleCharacterSeparator(char.Parse(outputTokens[i].Name)) || Checker.IsPossibleDoubleSeparator(char.Parse(outputTokens[i].Name)))
            {
                if (!Increment(ref i)) { return false; }

                return true;
            }

            return false;
        }

        private bool Expression(ref int i)
        {
            if (Term(ref i))
            {
                if (outputTokens[i].Name == "+" || outputTokens[i].Name == "-")
                {
                    if (!Increment(ref i)) { return false; }

                    if (Expression(ref i))
                    {
                        return true;
                    }
                }

                return true;
            }
            if (outputTokens[i].Name == "-")
            {
                if (!Increment(ref i)) { return false; }

                if (Factor(ref i))
                {
                    return true;
                }
            }

            return false;
        }

        private bool Factor(ref int i)
        {
            if (outputTokens[i].Name == "(")
            {
                if (!Increment(ref i)) { return false; }

                if (Expression(ref i))
                {
                    if (outputTokens[i].Name == ")")
                    {
                        if (!Increment(ref i)) { return false; }

                        return true;
                    }
                    else
                    {
                        errors.Add(" > Error on " + outputTokens[i].Row + " line!\tExpected ')'");
                    }
                }
            }
            if (IdnTable.Contains(outputTokens[i].Name))
            {
                if (!Increment(ref i)) { return false; }

                return true;
            }
            if (ConTable.Contains(outputTokens[i].Name))
            {
                if (!Increment(ref i)) { return false; }

                return true;
            }

            return false;
        }

        private bool Term(ref int i)
        {
            if (Factor(ref i))
            {
                if (outputTokens[i].Name == "*" || outputTokens[i].Name == "/")
                {
                    if (!Increment(ref i)) { return false; }

                    if (/*Factor*/Term(ref i))
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        private bool DeclarationsList(ref int i)
        {
            if (Declaration(ref i))
            {
                while (outputTokens[i].Name == ";")
                {
                    if (!Increment(ref i)) { return false; }

                    if (!Declaration(ref i))
                    {
                        errors.Add(" > Error on " + outputTokens[i].Row + " line!\tExpected declaration!");
                        return false;
                    }
                }

                return true;
            }
            else
            {
                errors.Add(" > Error on " + outputTokens[i].Row + " line!\tExpected declaration!");
                return false;
            }
        }

        private bool Declaration(ref int i)
        {
            if (Type(ref i))
            {
                if (VariablesList(ref i))
                {
                    return true;
                }
            }

            return false;
        }

        private bool VariablesList(ref int i)// TODO: by daclarations
        {
            if (IdnTable.Contains(outputTokens[i].Name))
            {
                if (!Increment(ref i)) { return false; }

                if (outputTokens[i].Name == ",")
                {
                    if (!Increment(ref i)) { return false; }

                    return VariablesList(ref i);
                }

                return true;
            }
            else
            {
                errors.Add(" > Error on " + outputTokens[i].Row + " line!\tExepted name of variable!");
                return false;
            }
        }

        private bool Type(ref int i)
        {
            if (Checker.IsType(outputTokens[i].Name))
            {
                if (!Increment(ref i)) { return false; }
                return true;
            }

            return false;
        }
    }
}
