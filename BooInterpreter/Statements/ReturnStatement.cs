using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class ReturnStatement : Statement
    {
        public ReturnStatement()
        {

        }

        public Expression ReturnValue { get; set; }

        public override string ToString()
        {
            return $"{TokenLiteral} {ReturnValue};";
        }
    }
}
