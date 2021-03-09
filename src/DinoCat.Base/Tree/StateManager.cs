using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinoCat.Base.Tree
{
    public class StateManager
    {
        private object sync = new object();
        private Action<Action> scheduleUpdate;
        private bool updateScheduled;
        private List<Node> pending = new List<Node>();

        public StateManager(Action<Action> scheduleUpdate)
        {
            this.scheduleUpdate = scheduleUpdate;
        }

        public void Invalidate(Node node)
        {
            bool scheduled;
            lock (sync)
            {
                pending.Add(node);
                scheduled = updateScheduled;
            }

            if (!scheduled)
                ScheduleUpdate();
        }

        private void ScheduleUpdate() => scheduleUpdate(this.Update);

        private void Update()
        {
            List<Node> work;
            lock (sync)
            {
                if (!updateScheduled)
                    return;

                work = pending;
                pending = new List<Node>();
                updateScheduled = false;
            }

            // Enforce top down update, because nodes can remove their children
            pending.Sort((a, b) => a.Depth.CompareTo(b.Depth));
            foreach (var node in pending)
                node.UpdateState();
        }
    }
}
