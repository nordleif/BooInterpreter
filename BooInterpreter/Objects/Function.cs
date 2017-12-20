using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter.Objects
{
    public class Function : Object
    {
        public Function()
        {

        }

        public BlockStatement Body { get; set; }

        public Environment Environment { get; set; }

        public Identifier[] Parameters { get; set; }

        public override ObjectType Type => ObjectType.Function;

        public override string ToString()
        {
            return $"fn({string.Join(", ", Parameters?.Select(p => p.ToString()))}){{\r\n{Body}\r\n}}";
        }
    }
}
