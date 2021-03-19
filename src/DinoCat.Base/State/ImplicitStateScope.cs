using System;
using System.ComponentModel;

namespace DinoCat.State
{
    public sealed class ImplicitStateScope<T> : IImplicitStateScope, INotifyPropertyChanged, IDisposable
    {
        private Func<T> exec;
        private IDisposable? shutdown;
        public event PropertyChangedEventHandler? PropertyChanged;

        public ImplicitStateScope(Func<T> exec) => this.exec = exec;

        public T Execute
        {
            get
            {
                T result;
                shutdown?.Dispose();
                using (var scope = ImplicitStateTracker.Push(this))
                {
                    result = exec();
                    shutdown = scope.Finalize();
                }
                return result;
            }
        }

        public void OnPropertyChanged(object? sender, PropertyChangedEventArgs args) =>
            PropertyChanged?.Invoke(this, PropertyChangedHelper.All);

        public void Dispose()
        {
            shutdown?.Dispose();
            shutdown = null;
        }
    }
}
