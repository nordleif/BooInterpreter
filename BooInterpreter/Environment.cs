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
        private Environment m_outer;
        private Dictionary<string, Object> m_store = new Dictionary<string, Object>();

        public Environment()
        {

        }

        public Environment(Environment outer)
        {
            if (outer == null)
                throw new ArgumentNullException(nameof(outer));

            m_outer = outer;
        }

        public Object Get(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (m_store.TryGetValue(name, out var value))
                return value;
            else
                return m_outer?.Get(name);
        }

        public void Set(string name, Object value)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (value == null)
                throw new ArgumentNullException(nameof(value));

            m_store[name] = value;
        }
    }
}
