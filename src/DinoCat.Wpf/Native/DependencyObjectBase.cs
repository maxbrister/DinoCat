using DinoCat.Elements;
using DinoCat.State;
using DinoCat.Wpf.Native.Internal;
using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Data;

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
            {
                if (pair.Value is BindingElementBase dinoBinding)
                {
                    var oldBinding = BindingOperations.GetBinding(dobj, pair.Key);
                    if (!(oldBinding?.Source is BindingAdapter adapter && adapter.Binding == dinoBinding))
                    {
                        var wpfBinding = new Binding("Value");
                        wpfBinding.Mode = BindingMode.OneWay;
                        wpfBinding.Source = new BindingAdapter(dinoBinding);
                        BindingOperations.SetBinding(dobj, pair.Key, wpfBinding);
                    }
                }
                else if (pair.Value is ImplicitStateScope<object> implicitState)
                {
                    var oldBinding = BindingOperations.GetBinding(dobj, pair.Key);
                    if (!(oldBinding?.Source is ImplicitBindingAdapter adapter && adapter.Binding == implicitState))
                    {
                        var wpfBinding = new Binding("Value");
                        wpfBinding.Mode = BindingMode.OneWay;
                        wpfBinding.Source = new ImplicitBindingAdapter(implicitState);
                        BindingOperations.SetBinding(dobj, pair.Key, wpfBinding);
                    }
                }
                else if (pair.Value is Element e)
                {
                    if (dobj.GetValue(pair.Key) is Host host)
                        host.RootElement = () => e;
                    else
                        dobj.SetValue(pair.Key, new Host(() => e));
                }
                else
                    dobj.SetValue(pair.Key, pair.Value);
            }

            foreach (var dp in previousLocal.Keys)
                if (!LocalValues.ContainsKey(dp))
                {
                    BindingOperations.ClearBinding(dobj, dp);
                    dobj.ClearValue(dp);
                }

            foreach (var prev in previousOperations)
                prev.Unapply(dobj);

            foreach (var next in Operations)
                next.Apply(dobj);
        }

        private class BindingAdapter : INotifyPropertyChanged, IDisposable
        {
            public event PropertyChangedEventHandler? PropertyChanged;
            private Host? host;

            public BindingAdapter(BindingElementBase binding)
            {
                Binding = binding;
                binding.PropertyChanged += Binding_PropertyChanged;
            }

            public BindingElementBase Binding { get; }

            public object Value
            {
                get
                {
                    var value = Binding.Value;
                    if (value is Element element)
                    {
                        host ??= new();
                        host.RootElement = () => element;
                        return host;
                    }
                    else
                    {
                        host = null;
                        return value;
                    }
                }
            }

            public void Dispose() => Binding.PropertyChanged -= Binding_PropertyChanged;
            
            private void Binding_PropertyChanged(object? sender, PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);
        }

        private class ImplicitBindingAdapter : INotifyPropertyChanged, IDisposable
        {
            public event PropertyChangedEventHandler? PropertyChanged;
            private Host? host;

            public ImplicitBindingAdapter(ImplicitStateScope<object> binding)
            {
                Binding = binding;
                binding.PropertyChanged += Binding_PropertyChanged;
            }

            public ImplicitStateScope<object> Binding { get; }

            public object Value
            {
                get
                {
                    var value = Binding.Execute;
                    if (value is Element element)
                    {
                        host ??= new();
                        host.RootElement = () => element;
                        return host;
                    }
                    else
                    {
                        host = null;
                        return value;
                    }
                }
            }

            public void Dispose() => Binding.PropertyChanged -= Binding_PropertyChanged;
            
            private void Binding_PropertyChanged(object? sender, PropertyChangedEventArgs args) => PropertyChanged?.Invoke(this, args);
        }
    }

    namespace Internal
    {
        public class DependencyObjectState<TSubclass> : INotifyPropertyChanged where TSubclass : DependencyObject, new()
        {
            public event PropertyChangedEventHandler? PropertyChanged;

            public DependencyObjectState()
            {
                // Ugly way to supress unused warning
                PropertyChanged?.Invoke(this, PropertyChangedHelper.All);
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
