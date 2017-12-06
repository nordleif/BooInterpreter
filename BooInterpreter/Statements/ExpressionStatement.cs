using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class ExpressionStatement : Statement
    {
        public ExpressionStatement()
        {

        }

        public Expression Expression { get; set; }

        public override string ToString()
        {
            return $"{Expression}";
        }
    }
}
