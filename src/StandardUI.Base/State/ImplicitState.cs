using Microsoft.StandardUI.Elements;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.StandardUI.State
{
    public static class ImplicitState
    {
        public static InjectState<ImplicitState<T>> Inject<T>(Func<ImplicitState<T>, Element> exec) where T: new() =>
            new InjectState<ImplicitState<T>>(
                () => new(),
                state => new ScopeElement(() => exec(state)));
    }

    public class ImplicitState<T> : INotifyPropertyChanged, IDisposable
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private int disposed;
        private T v;

        public ImplicitState() => v = Activator.CreateInstance<T>();
        public ImplicitState(T v) => this.v = v;

        public static implicit operator T(ImplicitState<T> s) => s.Value;

        public T Value
        {
            get
            {
                ImplicitStateTracker.OnReferenced(this);
                return v;
            }

            set
            {
                v = value;
                PropertyChanged?.Invoke(this, PropertyChangedHelper.All);
            }
        }

        public void Dispose()
        {
            if (v is IDisposable d && Interlocked.CompareExchange(ref disposed, 1, 0) == 0)
                d.Dispose();
        }

        public override string ToString() => Value?.ToString() ?? "";
    }
}
