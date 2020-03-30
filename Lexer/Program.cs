using System;
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

            string input = Console.ReadLine();

            List<Lexer.ResultToken> Tokens = lexer.GetTokens(input);

            foreach (var item in Tokens)
            {
                Console.Write("[" + item.name + ",\"" + item.output + "\"] ");
            }

            RecursiveParser.Tree Tree = recursiveParser.GetTree(Tokens);
            Console.WriteLine();
            Console.WriteLine(Tree.Nodes.Show());


        }

        

    }
}
