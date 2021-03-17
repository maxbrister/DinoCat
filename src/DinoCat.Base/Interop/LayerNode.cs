using DinoCat.Drawing;
using DinoCat.Elements;
using DinoCat.Tree;

namespace DinoCat.Interop
{
    public abstract class LayerNode<TElement> : NodeBase<TElement>, ILayerNode where TElement: Element
    {
        protected LayerNode(Node? parent, Context context, TElement element) : base(parent, context, element) { }

        public abstract ILayer Layer { get; }

        protected sealed override Size ArrangeOverride(Size availableSize)
        {
            var size = ArrangeOverrideImpl(availableSize);
            Layer.OnArrange(size);
            return size;
        }

        // OverrideImpl??? how else do we do this???
        protected abstract Size ArrangeOverrideImpl(Size availableSize);

        protected override void RenderOverride(IDrawingContext context) => Layer.OnRender(context);

        public virtual void RenderLayer(IDrawingContext context)
        {
            foreach (var child in Children)
                child.Render(context);
        }
    }
}
