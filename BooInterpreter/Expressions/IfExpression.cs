using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public class IfExpression : Expression
    {
        public IfExpression()
        {

        }

        public Expression Condition { get; set; }

        public BlockStatement Consequence { get; set; }

        public BlockStatement Alternative { get; set; }

        public override string ToString()
        {
            var text = $"if {Condition} {Consequence}";
            if (Alternative != null)
                text += $" else {Alternative}";
            return text;
        }
    }
}
