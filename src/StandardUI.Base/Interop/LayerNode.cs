using DinoCat.Drawing;
using DinoCat.Elements;
using DinoCat.Tree;

namespace DinoCat.Interop
{
    public abstract class LayerNode<TElement> : NodeBase<TElement>, ILayerNode where TElement : Element
    {
        protected LayerNode(Node? parent, Context context, TElement element) : base(parent, context, element) { }

        public abstract ILayer Layer { get; }

        protected sealed override (Size, float?) ArrangeOverride(Size availableSize)
        {
            var result = ArrangeOverrideImpl(availableSize);
            Layer.OnArrange(result);
            return result;
        }

        // OverrideImpl??? how else do we do this???
        protected abstract (Size, float?) ArrangeOverrideImpl(Size availableSize);

        public override void Dispose()
        {
            base.Dispose();
            Layer.Dispose();
        }

        protected override void RenderOverride(DrawingContext context) => Layer.OnRender(context);

        public virtual void RenderLayer(DrawingContext context)
        {
            foreach (var child in Children)
                child.Render(context);
        }
    }
}
