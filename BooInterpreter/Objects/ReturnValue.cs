using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter.Objects
{
    public class ReturnValue
    {
        public ReturnValue()
        {

        }

        public ObjectType Type => ObjectType.ReturnValue;

        public object Value { get; set; }

        public override string ToString()
        {
            return $"{Value}".ToLower();
        }
    }
}
