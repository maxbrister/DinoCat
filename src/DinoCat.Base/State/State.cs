using DinoCat.Elements;
using System;
using System.ComponentModel;
using System.Threading;

namespace DinoCat.State
{
    public static class State
    {
        public static InjectState<State<T>> Inject<T>(Func<T, Action<T>, Element> callback) where T : new() =>
            new InjectState<State<T>>(() => new(), state => callback(state.Value, newState => state.Value = newState));
        public static InjectState<State<T>> Inject<T>(Func<T> newState, Func<T, Action<T>, Element> callback) =>
            new InjectState<State<T>>(() => new State<T>(newState()),
                state => callback(state.Value, newState => state.Value = newState));
        public static InjectState<State<T>> UnsafeInject<T>(Func<State<T>, Element> callback) where T : new() =>
            new UnsafeInjectState<State<T>>(() => new(), callback);
        public static InjectState<State<T>> UnsafeInject<T>(Func<T> newState, Func<State<T>, Element> callback) =>
            new UnsafeInjectState<State<T>>(() => new State<T>(newState()), callback);
    }

    public sealed class State<T> : INotifyPropertyChanged, IDisposable
    {
        private int disposed;
        public event PropertyChangedEventHandler? PropertyChanged;

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
