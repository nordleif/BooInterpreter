using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class FunctionLiteral : Expression
    {
        public FunctionLiteral()
        {

        }

        public Identifier[] Parameters { get; set; }

        public BlockStatement Body { get; set; }

        public override string ToString()
        {
            return $"{TokenLiteral}({string.Join(", ", Parameters?.Select(p => p.ToString()))}){Body}";
        }
    }
}
