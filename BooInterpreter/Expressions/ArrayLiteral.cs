using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class ArrayLiteral : Expression
    {
        public ArrayLiteral()
        {

        }

        public Expression[] Elements { get; set; }

        public override string ToString()
        {
            return $"[{string.Join(", ", Elements?.Select(e => e.ToString()))}]";
        }
    }
}
