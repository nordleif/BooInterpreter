using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter.Objects
{
    public class Null
    {
        public Null()
        {

        }

        public ObjectType Type => ObjectType.Null;

        public override string ToString()
        {
            return $"null";
        }
    }
}
