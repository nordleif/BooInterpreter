using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class BlockStatement : Statement
    {
        public BlockStatement()
        {

        }

        public Statement[] Statements { get; set; }

        public override string ToString()
        {
            return string.Join("", Statements?.Select(s => s.ToString()));
        }
    }
}
