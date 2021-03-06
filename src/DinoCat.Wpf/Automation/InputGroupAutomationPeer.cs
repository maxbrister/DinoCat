using DinoCat.Elements;
using System.Windows.Automation.Peers;

namespace DinoCat.Wpf.Automation
{
    internal class InputGroupAutomationPeer : FrameworkElementAutomationPeer
    {
        private InputNode node;

        public InputGroupAutomationPeer(Layer owner, InputNode node) : base(owner) =>
            this.node = node;

        public Input Input => node.Element;

        protected override string GetNameCore() => node.Description;

        public override object GetPattern(PatternInterface patternInterface)
        {
            switch (patternInterface)
            {
                case PatternInterface.Invoke:
                    if (Input.Invoke != null)
                        return new InvokeProvider(this);
                    break;
            }

            return base.GetPattern(patternInterface);
        }

        protected override AutomationControlType GetAutomationControlTypeCore() =>
            (AutomationControlType)Input.ControlType;

        protected override string GetClassNameCore() =>
            Input.AutomationClassName;
    }
}
