using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class LetStatement : Statement
    {
        public LetStatement()
        {

        }

        public Token Token { get; set; }

        public Identifier Name { get; set; }

        public Expression Value { get; set; }

        public override string TokenLiteral => Token?.Literal;
    }
}
