using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class BooleanLiteral : Expression
    {
        public BooleanLiteral()
        {

        }

        public bool Value { get; set; }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}
