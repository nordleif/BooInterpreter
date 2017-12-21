using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class IndexExpression : Expression
    {
        public IndexExpression()
        {

        }

        public Expression Left { get; set; }

        public Expression Index { get; set; }

        public override string ToString()
        {
            return $"({Left})[{Index}]";
        }
    }
}
