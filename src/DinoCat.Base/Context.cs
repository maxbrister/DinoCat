using DinoCat.Interop;
using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DinoCat
{
    public class Context
    {
        private Context? parent;
        private Dictionary<Type, object> values;
        private StateManager stateManager;

        public Context(StateManager stateManager, ILayer layer, Dictionary<Type, object> initialContext)
        {
            values = new Dictionary<Type, object>();
            this.stateManager = stateManager;
            Layer = layer;
            values = initialContext;
        }

        public Context(Context parent, Dictionary<Type, object> newValues)
        {
            this.parent = parent;
            stateManager = parent.stateManager;
            Layer = parent.Layer;
            values = newValues;
        }

        public Context(Context parent, ILayer layer) : this(parent, new Dictionary<Type, object>())
        {
            Layer = layer;
        }

        public Context With<V>(V value) where V : notnull =>
            new Context(this, new Dictionary<Type, object> { { typeof(V), value } });

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

        public T? TryGet<T>() where T: class
        {
            if (values.TryGetValue(typeof(T), out var v))
                return (T)v;

            if (parent == null)
                return null;

            var value = parent.TryGet<T>();
            if (value != null)
                values[typeof(T)] = value;
            return value;
        }

        public ILayer Layer { get; }

        public void InvalidateState(int depth, Action update) =>
            stateManager.Invalidate(depth, update);

        public void InvalidateLayout() => Layer.InvalidateLayout();

        public void InvalidateRender() => Layer.InvalidateRender();
    }
}
