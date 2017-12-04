using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class Program : Node
    {
        public Program()
        {

        }

        public Statement[] Statements { get; set; }

        public override string TokenLiteral => Statements?.FirstOrDefault()?.TokenLiteral ?? string.Empty;
    }
}
