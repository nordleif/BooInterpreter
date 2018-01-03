using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class WhileExpression : Expression
    {
        public WhileExpression()
        {

        }

        public Expression Condition { get; set; }

        public BlockStatement Statement { get; set; }

        public override string ToString()
        {
            return $"while {Condition} {Statement}";
        }
    }
}
