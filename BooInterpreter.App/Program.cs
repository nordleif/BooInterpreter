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
                var evaluated = evaluator.Eval(program, environment);

                if (evaluated != null)
                    Console.WriteLine(evaluated);
            }
        }
    }
}
