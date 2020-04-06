using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    class RecursiveParser
    {
        List<Lexer.ResultToken> Tokens;
        int CurIn = 0;

        public class Tree //класс дерева
        {
            public Node Nodes;

            public Tree(Node node)
            {
                this.Nodes = node;
            }

            public class Node //базовый класс для ноды дерева
            {
                public enum OperatorType
                {
                    Plus,
                    Minus,
                    Mult,
                    Div,
                    Exp
                }

                public virtual string Show() //функция вывода содержимого ноды
                {
                    return "()";
                }
            }

            public class BinOperator : Node //нода бинарного оператора
            {
                public Node Left;
                public Node Right;
                public OperatorType Operator;

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

            public class Identifier : Node //нода строчного идентификатора 
            { 
                public string Value;

                public Identifier(string val)
                {
                    this.Value = val;
                }

                public override string Show()
                {
                    return "(" + "\"Id\" " + Value.ToString() + ")";
                }
            }

            public class Number : Node //нода числа (все приводим к double)
            {
                public double Value;

                public Number(double val)
                {
                    this.Value = val;
                }

                public override string Show()
                {
                    return "(" + "\"Number\" " + Value.ToString() + ")";
                }
            }

            public class UnarMinus : Node //нода унарного минуса 
            {
                public Node Value;

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

        public RecursiveParser(List<Lexer.ResultToken> tokens)
        {
            this.Tokens = tokens;
            CurIn = 0;
        }

        public Tree GetTree(List<Lexer.ResultToken> tokens) //получение дерева из списка токенов
        {
            Tree comp = ParseE(CurIn);
            if (CurIn > Tokens.Count - 1)
                return comp;
            else
                throw new ParserException("конец ввода", Tokens, CurIn);

        }

        //далее функции разборов нетерминалов по таблице 
        
        Tree ParseE(int index)
        {
            return ParseE2(ParseT(index));
        }

        Tree ParseE2(Tree comp)
        {
            if (CurIn > Tokens.Count - 1)
                return comp;
            else
            {
                switch (Tokens[CurIn].name)
                {
                    case "Operator":
                        {
                            if (Tokens[CurIn].output == "+")
                            {
                                Tree temp = ParseT(CurIn++);

                                Tree next;
                                next = new Tree(new Tree.BinOperator(Tree.Node.OperatorType.Plus,
                                                                                    comp.Nodes,
                                                                                    temp.Nodes));
                                return ParseE2(next);
                            }

                            else if (Tokens[CurIn].output == "-")
                            {
                                Tree temp = ParseT(CurIn++);

                                Tree next;
                                next = new Tree(new Tree.BinOperator(Tree.Node.OperatorType.Minus,
                                                                                    comp.Nodes,
                                                                                    temp.Nodes));
                                return ParseE2(next);
                            }
                        }
                        break;

                    default: return comp;
                }
                return comp;
            }
        }

        Tree ParseT(int index)
        {
            return ParseT2(ParseF(index));
        }

        Tree ParseT2(Tree comp)
        {
            if (CurIn > Tokens.Count - 1)
                return comp;
            else
            {
                switch (Tokens[CurIn].name)
                {
                    case "Operator":
                        {
                            if (Tokens[CurIn].output == "*")
                            {
                                Tree temp = ParseT(CurIn++);

                                Tree next;
                                next = new Tree(new Tree.BinOperator(Tree.Node.OperatorType.Mult,
                                                                                    comp.Nodes,
                                                                                    temp.Nodes));
                                return ParseE2(next);
                            }

                            else if (Tokens[CurIn].output == "/")
                            {
                                Tree temp = ParseT(CurIn++);

                                Tree next;
                                next = new Tree(new Tree.BinOperator(Tree.Node.OperatorType.Div,
                                                                                    comp.Nodes,
                                                                                    temp.Nodes));
                                return ParseE2(next);
                            }
                        }
                        break;

                    default: return comp;
                }
                return comp;
            }
        }

        Tree ParseF(int index)
        {
            return ParseF2(ParseV(index));
        }

        Tree ParseF2(Tree comp)
        {
            if (CurIn > Tokens.Count - 1)
                return comp;
            else
            {
                switch (Tokens[CurIn].name)
                {
                    case "Operator":
                        {
                            if (Tokens[CurIn].output == "^")
                            {
                                Tree temp = ParseT(CurIn++);

                                Tree next;
                                next = new Tree(new Tree.BinOperator(Tree.Node.OperatorType.Exp,
                                                                                    comp.Nodes,
                                                                                    temp.Nodes));
                                return ParseE2(next);
                            }
                        }
                        break;

                    default: return comp;
                }
                return comp;
            }           
        }

        Tree ParseV(int index)
        {
            if (CurIn > Tokens.Count - 1)
                throw new ParserException("(, Id, Number или -", Tokens, CurIn);
            switch (Tokens[CurIn].name)
            {
                case "Lparen":
                    {
                        Tree temp = ParseE(CurIn++);

                        if ((CurIn <= Tokens.Count - 1) && Tokens[CurIn].name == "Rparen")
                        {
                            Tree next;
                            CurIn++;
                            next = temp;
                            return next;
                        }
                        else
                        {   
                            throw new ParserException(")", Tokens, CurIn);
                        }                       
                    }
                    break;

                case "Id":
                    {
                        Tree next;
                        string outStr = Tokens[CurIn].output;
                        CurIn++;
                        next = new Tree(new Tree.Identifier(outStr));
                        return next;
                    }
                    break;

                case "Number":
                    {
                        Tree next;
                        string outStr = Tokens[CurIn].output;
                        CurIn++;
                        var englishCulture = CultureInfo.GetCultureInfo("en-US");
                        next = new Tree(new Tree.Number(double.Parse(outStr, englishCulture)));
                        return next;
                    }
                    break;

                case "Operator":
                    {
                        if (Tokens[CurIn].output == "-")
                        {
                            Tree temp = ParseV(CurIn++);

                            Tree next;
                            CurIn++;
                            next = new Tree(new Tree.UnarMinus(temp.Nodes));
                            return next;
                        }

                    }
                    break;
            }

            throw new ParserException("(, Id, Number или -", Tokens, CurIn);
        }
    }

    class ParserException : Exception //класс исключения работы синтаксического анализатора
    {
        public ParserException(string mes, List<Lexer.ResultToken> tok, int ind):base(mes)
        {
            string ErrMes = "Ошибка: ожидалось \"" + mes + "\" но получен ";

            if(ind > tok.Count - 1)
                ErrMes += "конец ввода";
            else
                ErrMes += "\"" + tok[ind].output + "\"";

            Console.WriteLine(ErrMes);
        }     
    }

}
