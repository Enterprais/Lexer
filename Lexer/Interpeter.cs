using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    class Interpeter //класс парсера дерева полученного из синтаксического анализатора
    {
        Dictionary<string, double> Variable = new Dictionary<string, double>(); //словарь переменных для тестирования

        public Interpeter()
        {
            //заполнение словаря переменных
            Variable.Add("pi", 3.14);
            Variable.Add("test", 42);
            Variable.Add("abc", 123);
        }

        public double Run(RecursiveParser.Tree.Node tree) //функция рекурсивного разбора дерева, возращает итоговое значение
        {
            //проверка ноды на принадлежность бинарному оператору
            if(tree is RecursiveParser.Tree.BinOperator)
            {
                RecursiveParser.Tree.BinOperator oper = (RecursiveParser.Tree.BinOperator)tree;

                //выполнение оператора в зависимости от его типа
                if (oper.Operator == RecursiveParser.Tree.Node.OperatorType.Plus)
                    return Run(oper.Left) + Run(oper.Right);
                else if (oper.Operator == RecursiveParser.Tree.Node.OperatorType.Minus)
                    return Run(oper.Left) - Run(oper.Right);
                else if (oper.Operator == RecursiveParser.Tree.Node.OperatorType.Mult)
                    return Run(oper.Left) * Run(oper.Right);
                else if (oper.Operator == RecursiveParser.Tree.Node.OperatorType.Div)
                    return Run(oper.Left) / Run(oper.Right);
                else if (oper.Operator == RecursiveParser.Tree.Node.OperatorType.Exp)
                    return Math.Pow(Run(oper.Left),Run(oper.Right));
            }
            //проверка ноды на принадлежность идентификатору (переменной)
            else if (tree is RecursiveParser.Tree.Identifier)
            {
                string val = ((RecursiveParser.Tree.Identifier)tree).Value;
                if (Variable.ContainsKey(val)) //поиск переменной в словаре
                    return Variable[val];
                else
                    throw new InterException("Ошибка: Неизвестная переменная \"" + val + "\"");

            }
            //проверка ноды на принадлежность числу
            else if (tree is RecursiveParser.Tree.Number)
            {
                return ((RecursiveParser.Tree.Number)tree).Value;
            }
            //проверка ноды на принадлежность унарному минусу
            else if (tree is RecursiveParser.Tree.UnarMinus)
            {
                return -Run(tree);
            }
                
            throw new InterException("Ошибка: ошибка разбора дерева");
        }
    }

    class InterException : Exception //класс исключения ошибки интерпретации
    {
        public InterException(string mes):base(mes)
        {
            Console.WriteLine("Результат выполнения интерпретатора:");
            Console.WriteLine(Message);
        }
    }

}
