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

        public override Node CreateNode(Node? parent, Context context) =>
            new InputNode(parent, context, this);
    }

    public class InputNode : LayerNode<Input>
    {
        private Node child;
        private ILayer layer;

        public event EventHandler<EventArgs>? ElementChanged;

        public InputNode(Node? parent, Context context, Input element) : base(parent, context, element)
        {
            layer = context.Layer.AddChild(this);
            var childContext = new Context(Context, Layer);
            child = Element.Child.CreateNode(this, childContext);
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

        protected override void UpdateElement(Input oldElement, Context oldContext)
        {
            var childContext = child.Context;
            if (oldContext != Context)
            {
                if (Context.Layer != oldContext.Layer)
                {
                    layer.Dispose();
                    layer = Context.Layer.AddChild(this);
                }


                childContext = new Context(Context, Layer);
            }

            child = child.UpdateElement(Element.Child, childContext);

            if (oldElement != Element)
                ElementChanged?.Invoke(this, EventArgs.Empty);
        }

        protected override (Size, float?) ArrangeOverrideImpl(Size availableSize) => child.Arrange(availableSize);
    }
}
