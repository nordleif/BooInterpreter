using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter.Objects
{
    public class Array : Object
    {
        public Array()
        {

        }

        public Object[] Elements { get; set; }

        public override ObjectType Type => ObjectType.Array;

        public override string ToString()
        {
            return $"[{string.Join(", ", Elements?.Select(e => e.ToString()))}]";
        }
    }
}
