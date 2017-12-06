using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class IntegerLiteral : Expression
    {
        public IntegerLiteral()
        {

        }

        public Int64 Value { get; set; }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}
