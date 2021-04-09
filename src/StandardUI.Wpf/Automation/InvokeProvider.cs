using System;
using System.Windows.Automation.Provider;

namespace Microsoft.StandardUI.Wpf.Automation
{
    internal class InvokeProvider : IInvokeProvider
    {
        private InputGroupAutomationPeer peer;
        public InvokeProvider(InputGroupAutomationPeer peer) => this.peer = peer;

        public void Invoke()
        {
            if (peer.Input.Invoke is Action invoke)
                invoke();
        }
    }
}
