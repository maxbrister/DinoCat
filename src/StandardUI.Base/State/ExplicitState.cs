using Microsoft.StandardUI.Elements;
using System;
using System.ComponentModel;
using System.Threading;

namespace Microsoft.StandardUI.State
{
    public static class ExplicitState
    {
        public static InjectState<ExplicitState<T>> Inject<T>(Func<ExplicitState<T>, Element> callback) =>
            new InjectState<ExplicitState<T>>(() => new(), callback);
    }

    public sealed class ExplicitState<T> : INotifyPropertyChanged, IDisposable
    {
        private int disposed;
        public event PropertyChangedEventHandler? PropertyChanged;

        private T v;

        public ExplicitState() => this.v = Activator.CreateInstance<T>();
        public ExplicitState(T v) => this.v = v;

        public T Value
        {
            internal get => v;
            set
            {
                v = value;
                PropertyChanged?.Invoke(this, PropertyChangedHelper.All);
            }
        }

        public void Set(Func<T, T> set) => Value = set(Value);

        public BindingElement<ExplicitState<T>, O> Bind<O>(Func<T, O> eval) where O: notnull =>
            new BindingElement<ExplicitState<T>, O>(this, () => eval(Value));

        public void Dispose()
        {
            if (v is IDisposable d && Interlocked.CompareExchange(ref disposed, 1, 0) == 0)
                d.Dispose();
        }
    }
}
