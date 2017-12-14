using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter.Objects
{
    public abstract class Object
    {
        public abstract ObjectType Type { get; }
    }
}
