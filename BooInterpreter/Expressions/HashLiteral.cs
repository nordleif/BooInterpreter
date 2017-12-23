using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class HashLiteral : Expression
    {
        public HashLiteral()
        {

        }

        public Dictionary<Expression, Expression> Pairs { get; set; }

        public override string ToString()
        {
            return $"{{{string.Join(", ", Pairs?.Select(p => p.ToString()))}}}";
        }
    }
}
