using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public abstract class Statement : Node
    {
        public Statement()
        {

        }

        public Token Token { get; set; }

        public override string TokenLiteral => Token.Literal;
    }
}
