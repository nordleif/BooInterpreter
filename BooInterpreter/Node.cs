using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter
{
    public abstract class Node
    {
        public virtual string TokenLiteral => string.Empty;
    }
}
