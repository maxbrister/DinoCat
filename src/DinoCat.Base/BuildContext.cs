using DinoCat.Base.Tree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DinoCat.Base
{
    public class BuildContext
    {
        private BuildContext? parent;
        private Dictionary<Type, object> values;
        private StateManager stateManager;
        private Action invalidateLayout;
        private Action invalidateRender;

        public BuildContext(StateManager stateManager, Action invalidateLayout, Action invalidateRender)
        {
            values = new Dictionary<Type, object>();
            this.stateManager = stateManager;
            this.invalidateLayout = invalidateLayout;
            this.invalidateRender = invalidateRender;
        }

        public BuildContext(BuildContext parent, object[] newValues)
        {
            this.parent = parent;
            stateManager = parent.stateManager;
            invalidateLayout = parent.invalidateLayout;
            invalidateRender = parent.invalidateRender;
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

        public void InvalidateState(Node node) =>
            stateManager.Invalidate(node);

        public void InvalidateLayout() => invalidateLayout();

        public void InvalidateRender() => invalidateRender();
    }
}
