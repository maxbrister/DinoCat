using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.StandardUI.Tree
{
    public class StateManager
    {
        private object sync = new object();
        private Action<Action> scheduleUpdate;
        private bool updateScheduled;
        private List<(int, Action)> pending = new List<(int, Action)>();

        public StateManager(Action<Action> scheduleUpdate)
        {
            this.scheduleUpdate = scheduleUpdate;
        }

        public void Invalidate(int depth, Action update)
        {
            bool isScheduled;
            lock (sync)
            {
                pending.Add((depth, update));
                isScheduled = updateScheduled;
                updateScheduled = true;
            }

            if (!isScheduled)
                ScheduleUpdate();
        }

        private void ScheduleUpdate() => scheduleUpdate(this.Update);

        private void Update()
        {
            List<(int, Action)> work;
            lock (sync)
            {
                if (!updateScheduled)
                    return;

                work = pending;
                pending = new List<(int, Action)>();
                updateScheduled = false;
            }

            // Enforce top down update, because nodes can remove their children
            pending.Sort((a, b) => a.Item1.CompareTo(b.Item1));
            foreach (var node in work)
                node.Item2();
        }
    }
}
