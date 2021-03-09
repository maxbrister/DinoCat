using System;
using System.Collections.Generic;
using System.Linq;

namespace DinoCat.Base
{
    public class BuildContext
    {
        private BuildContext? parent;
        private Dictionary<Type, object> values;

        public BuildContext()
        {
            values = new Dictionary<Type, object>();
        }

        public BuildContext(BuildContext parent, object[] newValues)
        {
            this.parent = parent;
            values = newValues.ToDictionary(v => v.GetType());
        }

        public T Get<T>() where T : new()
        {
            if (values.TryGetValue(typeof(T), out var v))
            {
                return (T)v;
            }

            var value = parent != null ? parent.Get<T>() : new T();
#pragma warning disable CS8601 // Possible null reference assignment.
            values[typeof(T)] = value;
#pragma warning restore CS8601 // Possible null reference assignment.
            return value;
        }
    }
}
