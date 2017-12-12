using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            var prompt = ">>> ";
            while (true)
            {
                Console.Write(prompt);
                var text = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(text))
                    return;

                var lexer = new Lexer(text);
                var parser = new Parser(lexer);
                var program = parser.ParseProgram();
                
                if (parser.Errors.Length > 0)
                {
                    Console.WriteLine(string.Join("\r\n", parser.Errors));
                    continue;
                }

                var evaluator = new Evaluator();
                var evaluated = evaluator.Eval(program);

                if (evaluated != null)
                    Console.WriteLine(evaluated);
            }
        }
    }
}
