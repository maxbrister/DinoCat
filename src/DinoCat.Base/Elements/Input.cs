using DinoCat.Elements;
using DinoCat.Elements.Events;
using DinoCat.Interop;
using DinoCat.Tree;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DinoCat.Elements
{
    public class Input : Element
    {
        private string? automationClassName;
        private Action? invoke;

        public Input(
            ControlType controlType,
            Element child,
            Action? tap = null,
            Action<bool>? gotFocus = null,
            Action? lostFocus = null,
            Action<KeyEvent>? keyDown = null,
            Action<KeyEvent>? keyUp = null,
            Action? mouseEnter = null,
            Action? mouseExit = null,
            Action? hover = null,
            Action? invoke = null,
            string? automationClassName = null,
            string? description = null)
        {
            Child = child;
            ControlType = controlType;
            this.automationClassName = automationClassName;
            Tap = tap;
            GotFocus = gotFocus;
            LostFocus = lostFocus;
            KeyDown = keyDown;
            KeyUp = keyUp;
            MouseEnter = mouseEnter;
            MouseLeave = mouseExit;
            Hover = hover;
            this.invoke = invoke;
            Description = description;
        }

        public string? Description { get; }
        public Element Child { get; }
        public ControlType ControlType { get; }
        public string AutomationClassName
        {
            get
            {
                if (!string.IsNullOrEmpty(automationClassName))
                    return automationClassName!;
                return ControlType.ToString();
            }

        }
        public Action? Tap { get; }
        public Action<bool>? GotFocus { get; }
        public Action? LostFocus { get; }
        public Action<KeyEvent>? KeyDown { get; }
        public Action<KeyEvent>? KeyUp { get; }
        public Action? MouseEnter { get; }
        public Action? MouseLeave { get; }
        public Action? Hover { get; }
        public Action? Invoke => invoke ?? Tap;

        public override Node CreateNode(int depth, Context context) =>
            new InputNode(depth, context, this);
    }

    public class InputNode : LayerNode<Input>
    {
        private Node child;
        private ILayer layer;

        public event EventHandler<EventArgs>? ElementChanged;

        public InputNode(int parentDepth, Context context, Input element) : base(parentDepth, context, element)
        {
            layer = context.Layer.AddChild(this);
            var childContext = new Context(Context, Layer);
            child = Element.Child.CreateNode(parentDepth, childContext);
        }

        public override ILayer Layer => layer;

        public override IEnumerable<Node> Children
        {
            get
            {
                yield return child;
            }
        }

        public override string Description
        {
            get
            {
                var description = Element.Description;
                if (!string.IsNullOrEmpty(description))
                    return description!;
                return base.Description;
            }
        }

        protected override void UpdateElement(Input oldElement)
        {
            child = child.UpdateElement(Element.Child);
            ElementChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override Size ArrangeOverrideImpl(Size availableSize) => child.Arrange(availableSize);

        protected override void UpdateContextOverride(Context oldContext)
        {
            if (Context.Layer != oldContext.Layer)
            {
                layer.Dispose();
                layer = Context.Layer.AddChild(this);
            }

            var childContext = new Context(Context, Layer);
            child = child.UpdateElement(Element.Child, childContext);
        }
    }
}
