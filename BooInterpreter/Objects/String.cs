using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter.Objects
{
    public class String : Object
    {
        public String()
        {

        }

        public override ObjectType Type => ObjectType.String;

        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }
    }
}
