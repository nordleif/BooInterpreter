using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    class Program
    {
        #region Static Members

        static Program()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, e) => {
                try
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var assemblyName = new AssemblyName(e.Name);
                    var fileName = assemblyName.Name + ".dll";
                    var resources = assembly.GetManifestResourceNames().Where(s => s.EndsWith(fileName));
                    if (resources.Any())
                    {
                        var resourceName = resources.First();
                        using (var stream = assembly.GetManifestResourceStream(resourceName))
                        {
                            if (stream == null)
                                return null;

                            var buffer = new byte[stream.Length];
                            try
                            {
                                stream.Read(buffer, 0, buffer.Length);
                                return Assembly.Load(buffer);
                            }
                            catch (IOException)
                            {
                                return null;
                            }
                            catch (BadImageFormatException)
                            {
                                return null;
                            }
                        }
                    }
                }
                catch
                {

                }
                return null;
            };
        }

        #endregion

        static void Main(string[] args)
        {
            var prompt = ">>> ";
            var environment = new Environment();


            var text = @"
                        let map = fn(arr, f) {
                            let iter = fn(arr, accumulated) {
                                if (len(arr) == 0) {
                                    accumulated
                                } else {
                                    iter(rest(arr), push(accumulated, f(first(arr))))
                                }
                            }

                            iter(arr, []);
                        };

                        let a = [ 1, 2, 3, 4 ];
                        let double = fn(x) { x * 2 };


                        let reduce = fn(arr, initial, f) {
                            let iter = fn(arr, result) {
                                if (len(arr) == 0) {
                                    result
                                } else {
                                    iter(rest(arr), f(result, first(arr)));
                                }
                            };

                            iter(arr, initial);
                        };

                        let sum = fn(arr) {
                            reduce(arr, 0, fn(initial, el) { initial + el });
                        };


            ";

            var lexer = new Lexer(text);
            var parser = new Parser(lexer);
            var program = parser.ParseProgram();
            var evaluator = new Evaluator();
            var evaluated = evaluator.Eval(program, environment);

            while (true)
            {
                Console.Write(prompt);
                text = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(text))
                    continue;

                lexer = new Lexer(text);
                parser = new Parser(lexer);
                program = parser.ParseProgram();
                
                if (parser.Errors.Length > 0)
                {
                    Console.WriteLine(string.Join("\r\n", parser.Errors));
                    continue;
                }

                evaluator = new Evaluator();
                evaluated = evaluator.Eval(program, environment);

                if (evaluated != null)
                    Console.WriteLine(evaluated);
            }
        }
    }
}
