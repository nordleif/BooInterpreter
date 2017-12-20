using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BooInterpreter.Objects;
using Object = BooInterpreter.Objects.Object;

namespace BooInterpreter
{
    public class Environment
    {
        private Dictionary<string, Object> m_store = new Dictionary<string, Object>();

        public Environment()
        {

        }

        public Object Get(string name)
        {
            if (m_store.TryGetValue(name, out var value))
                return value;
            else
                return null;
        }

        public void Set(string name, Object value)
        {
            m_store[name] = value;
        }
    }
}
