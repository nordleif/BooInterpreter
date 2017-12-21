using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class StringLiteral : Expression
    {
        public StringLiteral()
        {

        }

        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
