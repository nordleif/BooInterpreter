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

        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}
