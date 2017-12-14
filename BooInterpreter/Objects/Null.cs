using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter.Objects
{
    public class Null : Object
    {
        public Null()
        {

        }

        public override ObjectType Type => ObjectType.Null;

        public override string ToString()
        {
            return $"null";
        }
    }
}
