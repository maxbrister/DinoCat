using System;

namespace Microsoft.StandardUI.Wpf
{
    internal class Guard : IDisposable
    {
        private Action dispose;
        public Guard(Action dispose) => this.dispose = dispose;

        public void Dispose() => dispose();
    }
}
