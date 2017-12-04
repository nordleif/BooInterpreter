using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class Identifier : Expression
    {
        public Identifier()
        {

        }

        public Token Token { get; set; }

        public string Value { get; set; }

        public override string TokenLiteral => Token?.Literal ?? string.Empty;
    }
}
