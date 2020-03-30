using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    class RecursiveParser
    {
        public class Tree
        {
            public Node Nodes;

            public Tree(Node node)
            {
                this.Nodes = node;
            }

            public class Node
            {
                public enum OperatorType
                {
                    Plus,
                    Minus,
                    Mult,
                    Div,
                    Exp
                }

                public virtual string Show()
                {
                    return "()";
                }
            }

            public class BinOperator : Node
            {
                Node Left;
                Node Right;
                OperatorType Operator;

                public BinOperator(OperatorType op, Node left, Node right)
                {
                    this.Operator = op;
                    this.Left = left;
                    this.Right = right;
                }

                public override string Show()
                {
                    return "(" + "\"" + Operator.ToString() + "\" "  + Left.Show() + " " + Right.Show() + ")";
                }
            }

            public class Identifier : Node
            {
                string Value;

                public Identifier(string val)
                {
                    this.Value = val;
                }

                public override string Show()
                {
                    return "(" + "\"Id\" " + Value.ToString() + ")";
                }
            }

            public class Number : Node
            {
                double Value;

                public Number(double val)
                {
                    this.Value = val;
                }

                public override string Show()
                {
                    return "(" + "\"Number\" " + Value.ToString() + ")";
                }
            }

            public class UnarMinus : Node
            {
                Node Value;

                public UnarMinus(Node tree)
                {
                    this.Value = tree;
                }

                public override string Show()
                {
                    return "(" + "\"UnarMinus\" " + Value.ToString() + ")";
                }

            }
        }

        public Tree GetTree(List<Lexer.ResultToken> tokens)
        {
            StepComp comp = ParseE(tokens);
            if (comp.results.Count == 0)
                return comp.tree;
            else
                throw new Exception("Сломалось");
                
        }

        struct StepComp
        {
            public List<Lexer.ResultToken> results;
            public Tree tree;
        }

        StepComp ParseE(List<Lexer.ResultToken> tokens)
        {
            return ParseE2(ParseT(tokens));
        }

        StepComp ParseE2(StepComp comp)
        {
            if (comp.results.Count == 0)
                return comp;
            else
            {
                switch (comp.results[0].name)
                {
                    case "Operator":
                        {
                            if (comp.results[0].output == "+")
                            {
                                StepComp temp = ParseT(comp.results.Skip(1).ToList());

                                StepComp next;
                                next.results = temp.results;
                                next.tree = new Tree(new Tree.BinOperator(Tree.Node.OperatorType.Plus,
                                                                                    comp.tree.Nodes,
                                                                                    temp.tree.Nodes));

                                return ParseE2(next);
                            }

                            else if (comp.results[0].output == "-")
                            {
                                StepComp temp = ParseT(comp.results.Skip(1).ToList());

                                StepComp next;
                                next.results = temp.results;
                                next.tree = new Tree(new Tree.BinOperator(Tree.Node.OperatorType.Minus,
                                                                                    comp.tree.Nodes,
                                                                                    temp.tree.Nodes));

                                return ParseE2(next);
                            }

                        }
                        break;

                    default: return comp;
                }
                return comp;
            }
        }

        StepComp ParseT(List<Lexer.ResultToken> tokens)
        {
            return ParseT2(ParseF(tokens));
        }

        StepComp ParseT2(StepComp comp)
        {
            if (comp.results.Count == 0)
                return comp;
            else
            {
                switch (comp.results[0].name)
                {
                    case "Operator":
                        {
                            if (comp.results[0].output == "*")
                            {
                                StepComp temp = ParseT(comp.results.Skip(1).ToList());

                                StepComp next;
                                next.results = temp.results;
                                next.tree = new Tree(new Tree.BinOperator(Tree.Node.OperatorType.Mult,
                                                                                    comp.tree.Nodes,
                                                                                    temp.tree.Nodes));
                                return ParseE2(next);
                            }

                            else if (comp.results[0].output == "/")
                            {
                                StepComp temp = ParseT(comp.results.Skip(1).ToList());

                                StepComp next;
                                next.results = temp.results;
                                next.tree = new Tree(new Tree.BinOperator(Tree.Node.OperatorType.Div,
                                                                                    comp.tree.Nodes,
                                                                                    temp.tree.Nodes));

                                return ParseE2(next);
                            }

                        }
                        break;

                    default: return comp;
                }
                return comp;
            }
        }

        StepComp ParseF(List<Lexer.ResultToken> tokens)
        {
            return ParseF2(ParseV(tokens));
        }

        StepComp ParseF2(StepComp comp)
        {
            if (comp.results.Count == 0)
                return comp;
            else
            {
                switch (comp.results[0].name)
                {
                    case "Operator":
                        {
                            if (comp.results[0].output == "^")
                            {
                                StepComp temp = ParseT(comp.results.Skip(1).ToList());

                                StepComp next;
                                next.results = temp.results;
                                next.tree = new Tree(new Tree.BinOperator(Tree.Node.OperatorType.Exp,
                                                                                    comp.tree.Nodes,
                                                                                    temp.tree.Nodes));


                                return ParseE2(next);
                            }
                        }
                        break;

                    default: return comp;
                }
                return comp;
            }
            
        }

        StepComp ParseV(List<Lexer.ResultToken> tokens)
        {
            switch (tokens[0].name)
            {
                case "Lparen":
                    {
                        StepComp temp = ParseE(tokens.Skip(1).ToList());

                        if (temp.results.Count != 0 && temp.results[0].name == "Rparen")
                        {
                            StepComp next;
                            next.results = temp.results.Skip(1).ToList();
                            next.tree = temp.tree;
                            return next;
                        }
                        else
                        {   
                            throw new Exception("Нет закрывающей скобки");
                        }
                        
                    }
                    break;

                case "Id":
                    {
                        StepComp next;
                        next.results = tokens.Skip(1).ToList();
                        next.tree = new Tree(new Tree.Identifier(tokens[0].output));
                        return next;
                    }
                    break;

                case "Number":
                    {
                        StepComp next;
                        next.results = tokens.Skip(1).ToList();
                        next.tree = new Tree(new Tree.Number(double.Parse(tokens[0].output)));
                        return next;
                    }
                    break;

                case "Operator":
                    {
                        if (tokens[0].output == "-")
                        {
                            StepComp temp = ParseV(tokens.Skip(1).ToList());

                            StepComp next;
                            next.results = temp.results;
                            next.tree = new Tree(new Tree.UnarMinus(temp.tree.Nodes));
                            return next;
                        }

                    }
                    break;
            }

            throw new Exception("LOLD");
        }
    }
}
