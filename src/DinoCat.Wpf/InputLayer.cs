using DinoCat.Elements;
using DinoCat.Wpf.Automation;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Input;

namespace DinoCat.Wpf
{
    internal class InputLayer : Layer
    {
        private InputGroupAutomationPeer? peer;
        private bool focusFromKeyboard = true;

        public InputLayer(Layer parent, InputNode node) : base(parent, node)
        {
            GotFocus += InputLayer_GotFocus;
            LostFocus += InputLayer_LostFocus;
            MouseEnter += InputLayer_MouseEnter;
            MouseLeave += InputLayer_MouseLeave;
            MouseDown += InputLayer_MouseDown;
            MouseUp += InputLayer_MouseUp;
            KeyDown += InputLayer_KeyDown;
            KeyUp += InputLayer_KeyUp;
            FocusVisualStyle = null;
            IsHitTestVisible = true;
            OnUpdated();
        }

        public override void OnUpdated()
        {
            base.OnUpdated();
            peer?.InvalidatePeer();
            Focusable = Input.GotFocus != null;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            peer ??= new InputGroupAutomationPeer(this, (InputNode)root!);
            return peer;
        }

        private void InputLayer_GotFocus(object sender, RoutedEventArgs e) =>
            Input.GotFocus?.Invoke(focusFromKeyboard);

        private void InputLayer_LostFocus(object sender, RoutedEventArgs e) =>
            Input.LostFocus?.Invoke();

        private void InputLayer_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e) =>
            Input.MouseEnter?.Invoke();

        private void InputLayer_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e) =>
            Input.MouseLeave?.Invoke();

        private void InputLayer_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && Input.Tap != null)
            {
                try
                {
                    focusFromKeyboard = false;
                    Focus();
                }
                finally
                {
                    focusFromKeyboard = true;
                }

                Mouse.Capture(this, CaptureMode.Element);
            }
        }

        private void InputLayer_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (Mouse.Captured == this)
            {
                Mouse.Capture(null);
                var p = e.GetPosition(this);
                if (new Rect(new Size((float)ActualWidth, (float)ActualHeight)).Contains(p.Into()))
                    Input.Tap?.Invoke();
            }
        }

        private void InputLayer_KeyDown(object sender, KeyEventArgs e)
        {
            var k = e.Into();
            Input.KeyDown?.Invoke(k);
            e.Handled = k.Handled;
        }

        private void InputLayer_KeyUp(object sender, KeyEventArgs e)
        {
            var k = e.Into();
            Input.KeyUp?.Invoke(k);
            e.Handled = k.Handled;
        }

        private Input Input => ((InputNode)root!).Element;
    }
}
