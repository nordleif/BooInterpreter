using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class PrefixExpression : Expression
    {
        public PrefixExpression()
        {

        }

        public string Operator { get; set; }

        public Expression Right { get; set; }

        public override string ToString()
        {
            return $"({Operator}{Right})";
        }
    }
}
