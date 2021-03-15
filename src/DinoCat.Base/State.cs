using DinoCat.Elements;
using System;
using System.Threading;

namespace DinoCat
{
    public static class State
    {
        public static InjectState<State<T>> Inject<T>(Func<State<T>, Element> callback) where T : new() =>
            new InjectState<State<T>>(() => new(), callback);
        public static InjectState<State<T>> Inject<T>(Func<T> newState, Func<State<T>, Element> callback) =>
            new InjectState<State<T>>(() => new State<T>(newState()), callback);
        public static InjectState<State<T>> UnsafeInject<T>(Func<State<T>, Element> callback) where T : new() =>
            new InjectState<State<T>>(() => new(), callback);
        public static InjectState<State<T>> UnsafeInject<T>(Func<T> newState, Func<State<T>, Element> callback) =>
            new InjectState<State<T>>(() => new State<T>(newState()), callback);
    }

    public sealed class State<T> : IState
    {
        private int disposed;
        public event EventHandler<EventArgs>? StateChanged;
        private T v;

        public static implicit operator T(State<T> state) => state.Value;

        public State() => this.v = Activator.CreateInstance<T>();

        public State(T v) => this.v = v;

        public T Value
        {
            get => v;
            set
            {
                v = value;
                StateChanged?.Invoke(this, EventArgs.Empty);
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
