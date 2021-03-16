using DinoCat.Wpf.Native.Internal;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Windows;

namespace DinoCat.Wpf.Native
{
    public abstract class DependencyObjectBase<TDependencyObject> : Elements.UnsafeControl<DependencyObjectState<TDependencyObject>> where TDependencyObject : DependencyObject, new()
    {
        public DependencyObjectBase()
        {
            LocalValues = ImmutableDictionary.Create<DependencyProperty, object>();
            Operations = ImmutableList.Create<Operation<TDependencyObject>>();
        }

        public DependencyObjectBase((DependencyProperty, object?)[] localValues, Operation<TDependencyObject>?[] operations)
        {
            LocalValues = localValues.Where(i => i.Item2 != null).ToImmutableDictionary(i => i.Item1, i => i.Item2!);
            Operations = operations
                .Where(op => op != null)
                .Select(op => op!.Value)
                .ToImmutableList();
        }

        public DependencyObjectBase(
            ImmutableDictionary<DependencyProperty, object> localValues,
            ImmutableList<Operation<TDependencyObject>> operations)
        {
            LocalValues = localValues;
            Operations = operations;
        }

        public ImmutableDictionary<DependencyProperty, object> LocalValues { get; }
        public ImmutableList<Operation<TDependencyObject>> Operations { get; }

        protected void Update(TDependencyObject dobj, Internal.DependencyObjectState<TDependencyObject> state)
        {
            var previousLocal = state.Previous?.LocalValues ?? ImmutableDictionary.Create<DependencyProperty, object>();
            var previousOperations = state.Previous?.Operations ?? ImmutableList.Create<Operation<TDependencyObject>>();
            state.Previous = this;

            foreach (var pair in LocalValues)
                dobj.SetValue(pair.Key, pair.Value);

            foreach (var dp in previousLocal.Keys)
                if (!LocalValues.ContainsKey(dp))
                    dobj.ClearValue(dp);

            foreach (var prev in previousOperations)
                prev.Unapply(dobj);

            foreach (var next in Operations)
                next.Apply(dobj);
        }
    }

    namespace Internal
    {
        public class DependencyObjectState<TSubclass> : IState where TSubclass : DependencyObject, new()
        {
            public event EventHandler<EventArgs> StateChanged
            {
                add { }
                remove { }
            }

            public void Dispose() { }

            public DependencyObjectBase<TSubclass>? Previous;
        }

        public struct Operation<TSubclass>
        {
            public Action<TSubclass> Apply;
            public Action<TSubclass> Unapply;
        }
    }
}
