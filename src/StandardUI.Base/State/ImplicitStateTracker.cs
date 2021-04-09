using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace DinoCat.State
{
    public class ImplicitStateTracker : IDisposable
    {
        private static ThreadLocal<List<(IImplicitStateScope, HashSet<INotifyPropertyChanged>)>> stack = new(() => new());
        private HashSet<INotifyPropertyChanged>? set;

        private ImplicitStateTracker(HashSet<INotifyPropertyChanged> set) => this.set = set;

        public static void OnReferenced(INotifyPropertyChanged state)
        {
            var current = stack.Value!.LastOrDefault();
            if (current.Item1 != null)
            {
                if (current.Item2.Add(state))
                    state.PropertyChanged += current.Item1.OnPropertyChanged;
            }
        }

        public static ImplicitStateTracker Push(IImplicitStateScope scope)
        {
            HashSet<INotifyPropertyChanged> set = new();
            stack.Value!.Add((scope, set));
            return new ImplicitStateTracker(set);
        }

        public IDisposable Finalize()
        {
            if (set == null)
                throw new Exception("Finalize can only be called once");

            var localStack = stack.Value!;
            int at = localStack.FindLastIndex(pair => pair.Item2 == set);
            if (at == -1)
                throw new Exception("Scope finalized on the wrong thread");
            if (at != localStack.Count - 1)
                Debug.Fail("Out of order scope finalization");

            var current = localStack.LastOrDefault();
            localStack.RemoveAt(localStack.Count - 1);
            var unregister = new Unregister(current.Item2, current.Item1);
            set = null;
            return unregister;
        }

        public void Dispose()
        {
            if (set != null)
                Finalize().Dispose();
        }

        private record Unregister(IEnumerable<INotifyPropertyChanged> Senders, IImplicitStateScope Scope) : IDisposable
        {
            public void Dispose()
            {
                foreach (var sender in Senders)
                    sender.PropertyChanged -= Scope.OnPropertyChanged;
            }
        }
    }
}
