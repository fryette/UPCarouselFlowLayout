using CoreGraphics;
using UIKit;

namespace UPCarouselFlowLayout
{
    internal struct LayoutState
    {
        public CGSize Size { get; }
        public UICollectionViewScrollDirection Direction { get; }

        public LayoutState(CGSize size, UICollectionViewScrollDirection direction)
        {
            Direction = direction;
            Size = size;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public bool Equals(LayoutState other)
        {
            return Size.Equals(other.Size) && Direction == other.Direction;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Size.GetHashCode() * 397) ^ Direction.GetHashCode();
            }
        }
    }
}