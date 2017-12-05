using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class ReturnStatement : Statement
    {
        public ReturnStatement()
        {

        }

        public Token Token { get; set; }

        public Expression ReturnValue { get; set; }

        public override string TokenLiteral => Token?.Literal;
    }
}
