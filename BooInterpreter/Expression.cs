using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public abstract class Expression : Node
    {
        public Expression()
        {

        }

        public Token Token { get; set; }

        public override string TokenLiteral => Token.Literal;
    }
}
