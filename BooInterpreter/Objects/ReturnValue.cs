using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter.Objects
{
    public class ReturnValue : Object
    {
        public ReturnValue()
        {

        }

        public override ObjectType Type => ObjectType.ReturnValue;

        public Object Value { get; set; }

        public override string ToString()
        {
            return $"{Value}".ToLower();
        }
    }
}
