using DinoCat.Base.Controls;
using DinoCat.Base.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DinoCat.Base.Interop
{
    public class Root
    {
        private Action<Action> scheduleUpdateState;
        private Func<Element> genRoot;
        private Element rootElement;
        private IState? rootState;
        private long lastUpdate;
        private long changeCount = 1;
        private BuildContext context = new BuildContext();

        public event EventHandler<EventArgs>? ArrangeInvalidated;
        public event EventHandler<EventArgs>? RenderInvalidated;

        public Root(Action<Action> scheduleUpdateState, Func<Element> genRoot)
        {
            this.scheduleUpdateState = scheduleUpdateState;
            this.genRoot = genRoot;

            rootElement = genRoot();
            rootElement.ArrangeInvalidated += RootElement_ArrangeInvalidated;
            rootElement.RenderInvalidated += RootElement_RenderInvalidated;
            UpdateState();
        }

        public Func<Element> GenRoot
        {
            get => genRoot;
            set
            {
                genRoot = value;
                Refresh();
            }
        }

        public Size Arrange(Size availableSpace) =>
            BoundRoot.Arrange(availableSpace);

        public void Render(IDrawingContext context) =>
            BoundRoot.Render(context);

        public void Refresh()
        {
            RootElement = genRoot();
            ArrangeInvalidated?.Invoke(this, EventArgs.Empty);
            RenderInvalidated?.Invoke(this, EventArgs.Empty);
            Interlocked.Increment(ref changeCount);
            UpdateState();
        }

        private Element RootElement
        {
            get => rootElement;
            set
            {
                if (rootElement != null)
                {
                    rootElement.ArrangeInvalidated -= RootElement_ArrangeInvalidated;
                    rootElement.RenderInvalidated -= RootElement_RenderInvalidated;
                }

                rootElement = value;

                if (rootElement != null)
                {
                    rootElement.ArrangeInvalidated += RootElement_ArrangeInvalidated;
                    rootElement.RenderInvalidated += RootElement_RenderInvalidated;
                }
            }
        }

        private BoundElement BoundRoot => new BoundElement(RootElement, RootState);

        private IState? RootState
        {
            get => rootState;
            set
            {
                if (rootState != value)
                {
                    if (rootState != null)
                    {
                        rootState.StateChanged -= RootState_StateChanged;
                    }

                    rootState = value;

                    if (rootState != null)
                    {
                        rootState.StateChanged += RootState_StateChanged;
                    }
                }
            }
        }

        private void UpdateState()
        {
            if (changeCount == lastUpdate)
                return;

            lastUpdate = changeCount;
            RootState = BoundRoot.CreateOrReuseState(context);
            BoundRoot.UpdateState(context);
        }

        private void RootState_StateChanged(object? sender, EventArgs e)
        {
            Interlocked.Increment(ref changeCount);
            scheduleUpdateState(this.UpdateState);
        }

        private void RootElement_ArrangeInvalidated(object? sender, EventArgs e) =>
            ArrangeInvalidated?.Invoke(this, e);

        private void RootElement_RenderInvalidated(object? sender, EventArgs e) =>
            RenderInvalidated?.Invoke(this, e);
    }
}
