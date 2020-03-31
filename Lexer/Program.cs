﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    class Program
    {
        static void Main(string[] args)
        {
            Lexer lexer = new Lexer();
            RecursiveParser recursiveParser = new RecursiveParser();
            Interpeter interpeter = new Interpeter();

            Console.WriteLine("По умолчанию заданы переменные: pi=3.14 abc=123 test=42");
            Console.WriteLine("Введите строку для разбора:");
            string input = Console.ReadLine();

            List<Lexer.ResultToken> Tokens = lexer.GetTokens(input);

            Console.WriteLine("Вывод лексического анализатора:");
            foreach (var item in Tokens)
            {
                Console.Write("[" + item.name + ",\"" + item.output + "\"] ");
            }

            Console.WriteLine("\nВывод синтаксического анализатора:");
            try
            {
                RecursiveParser.Tree Tree = recursiveParser.GetTree(Tokens);
                Console.WriteLine(Tree.Nodes.Show());
                double result = interpeter.Run(Tree.Nodes);
                Console.WriteLine("Результат выполнения интерпретатора:");
                Console.WriteLine(result);              
            }
            catch (Exception)
            {
                
            }
        }
    }
}
