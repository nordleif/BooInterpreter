using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter.Objects
{
    public class Hash : Object
    {
        public Hash()
        {

        }

        public Dictionary<Object, Object> Pairs { get; set; }

        public override ObjectType Type => ObjectType.Hash;

        public override string ToString()
        {
            return $"{string.Join(", ", Pairs?.Select(p => $"{p.Key} : {p.Value}"))}";
        }
    }
}
