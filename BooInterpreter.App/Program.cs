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
                //Console.Write(prompt);
                //var text = Console.ReadLine();
                //if (string.IsNullOrWhiteSpace(text))
                //    return;

                //var lexer = new Lexer(text);
                //while (true)
                //{
                //    var token = lexer.NextToken();
                //    if (token.Type == TokenType.EOF)
                //        break;
                //    Console.WriteLine($"{token.Type.ToString().PadRight(10)} {token.Literal}");
                //}

                Console.Write(prompt);
                var text = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(text))
                    return;

                var lexer = new Lexer(text);
                var parser = new Parser(lexer);
                var program = parser.ParseProgram();

                if (parser.Errors.Length > 0)
                    Console.WriteLine(string.Join("\r\n", parser.Errors));
                else
                    Console.WriteLine($"{program}");

            }
        }
    }
}
