using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public enum Precedence
    {
        Lowest,
        Equals,      // ==
        LessGreater, // > or <
        Sum,         // +
        Product,     // *
        Prefix,      // -X or !X
        Call,        // myFunction(X)
        Index,       // array[index]
    }
}
