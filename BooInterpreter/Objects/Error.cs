using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BooInterpreter.Objects
{
    public class Error : Object
    {
        public Error()
        {

        }

        public string Message { get; set; }

        public override ObjectType Type => ObjectType.Error;
        
        public override string ToString()
        {
            return $"{Message}";
        }
    }
}
