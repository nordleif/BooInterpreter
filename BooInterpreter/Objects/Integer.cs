using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter.Objects
{
    public class Integer
    {
        public Integer()
        {

        }

        public ObjectType Type => ObjectType.Integer;

        public Int64 Value { get; set; }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}
