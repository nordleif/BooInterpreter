using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter.Objects
{
    public class Builtin : Object
    {
        public Builtin()
        {

        }

        public Func<Object[], Object> Function { get; set; }

        public override ObjectType Type => ObjectType.Builtin;

        public override string ToString()
        {
            return "builtin function";
        }
    }
}
