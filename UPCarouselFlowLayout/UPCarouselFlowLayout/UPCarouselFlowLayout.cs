using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace UPCarouselFlowLayout
{
    [Register("UPCarouselFlowLayout")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    public class UPCarouselFlowLayout : UICollectionViewFlowLayout
    {
        public UPCarouselFlowLayout(IntPtr handle) : base(handle)
        {
            Initialize();
        }

        public UPCarouselFlowLayout()
        {
            Initialize();
        }

        [Export("sideItemScale"), Browsable(true)]
        public nfloat SideItemScale { get; set; }

        [Export("sideItemAlpha"), Browsable(true)]
        public nfloat SideItemAlpha { get; set; }

        [Export("sideItemShift"), Browsable(true)]
        public nfloat SideItemShift { get; set; }

        public void SetSpacingMode(UPCarouselFlowLayoutSpacingMode state, nfloat size)
        {
            _spacingMode = new UPCarouselFlowLayoutSpacingModeState(state, size);
        }

        private UPCarouselFlowLayoutSpacingModeState _spacingMode;

        private LayoutState _state = new LayoutState(CGSize.Empty, UICollectionViewScrollDirection.Horizontal);

        public override void PrepareLayout()
        {
            base.PrepareLayout();

            var currentState = new LayoutState(CollectionView.Bounds.Size, ScrollDirection);

            if (_state.Equals(currentState))
            {
                return;
            }

            SetupCollectionView();
            UpdateLayout();
            _state = currentState;
        }

        private void SetupCollectionView()
        {
            if (CollectionView == null)
            {
                return;
            }

            if (CollectionView.DecelerationRate != UIScrollView.DecelerationRateFast)
            {
                CollectionView.DecelerationRate = UIScrollView.DecelerationRateFast;
            }
        }

        private void UpdateLayout()
        {
            if (CollectionView == null)
            {
                return;
            }

            var collectionSize = CollectionView.Bounds.Size;
            var isHorizontal = ScrollDirection == UICollectionViewScrollDirection.Horizontal;

            var yInset = (collectionSize.Height - ItemSize.Height) / 2;
            var xInset = (collectionSize.Width - ItemSize.Width) / 2;
            SectionInset = new UIEdgeInsets(yInset, xInset, yInset, xInset);

            var side = isHorizontal ? ItemSize.Width : ItemSize.Height;
            var scaledItemOffset = (side - side * SideItemScale) / 2;

            switch (_spacingMode.Mode)
            {
                case UPCarouselFlowLayoutSpacingMode.Fixed:
                    MinimumLineSpacing = _spacingMode.Size - scaledItemOffset;
                    break;
                case UPCarouselFlowLayoutSpacingMode.Overlap:
                    var fullSizeSideItemOverlap = _spacingMode.Size + scaledItemOffset;
                    var inset = isHorizontal ? xInset : yInset;
                    MinimumLineSpacing = inset - fullSizeSideItemOverlap;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override bool ShouldInvalidateLayoutForBoundsChange(CGRect newBounds)
        {
            return true;
        }

        public override UICollectionViewLayoutAttributes[] LayoutAttributesForElementsInRect(CGRect rect)
        {
            return base.LayoutAttributesForElementsInRect(rect).Select(TransformLayoutAttributes).ToArray();
        }

        private UICollectionViewLayoutAttributes TransformLayoutAttributes(UICollectionViewLayoutAttributes attributes)
        {
            if (CollectionView == null)
            {
                return attributes;
            }

            var isHorizontal = ScrollDirection == UICollectionViewScrollDirection.Horizontal;
            var collectionCenter =
                isHorizontal ? CollectionView.Frame.Size.Width / 2 : CollectionView.Frame.Size.Height / 2;
            var offset = isHorizontal ? CollectionView.ContentOffset.X : CollectionView.ContentOffset.Y;
            var normalizedCenter = (isHorizontal ? attributes.Center.X : attributes.Center.Y) - offset;

            var maxDistance = (isHorizontal ? ItemSize.Width : ItemSize.Height) + MinimumLineSpacing;
            var distance = Math.Min(Math.Abs(collectionCenter - normalizedCenter), maxDistance);
            var ratio = (float)((maxDistance - distance) / maxDistance);

            var alpha = ratio * (1 - SideItemAlpha) + SideItemAlpha;
            var scale = ratio * (1 - SideItemScale) + SideItemScale;
            var shift = (1 - ratio) * SideItemShift;

            attributes.Alpha = alpha;
            attributes.Transform3D = CATransform3D.MakeScale(scale, scale, 1);
            attributes.ZIndex = (int)(alpha * 10);

            attributes.Center = isHorizontal
                ? new CGPoint(attributes.Center.X, attributes.Center.Y + shift)
                : new CGPoint(attributes.Center.X + shift, attributes.Center.Y);

            return attributes;
        }

        public override CGPoint TargetContentOffset(CGPoint proposedContentOffset, CGPoint scrollingVelocity)
        {
            if (CollectionView == null)
            {
                return base.TargetContentOffset(proposedContentOffset, scrollingVelocity);
            }

            var isHorizontal = ScrollDirection == UICollectionViewScrollDirection.Horizontal;

            var midSide = (isHorizontal ? CollectionView.Bounds.Size.Width : CollectionView.Bounds.Size.Height) / 2;
            var proposedContentOffsetCenterOrigin =
                (isHorizontal ? proposedContentOffset.X : proposedContentOffset.Y) + midSide;

            var layoutAttributes = LayoutAttributesForElementsInRect(CollectionView.Bounds);

            CGPoint targetContentOffset;

            if (isHorizontal)
            {
                var closest =
                    layoutAttributes.OrderBy(x => Math.Abs(x.Center.X - proposedContentOffsetCenterOrigin))
                        .FirstOrDefault() ?? new UICollectionViewLayoutAttributes();
                targetContentOffset = new CGPoint(Math.Floor(closest.Center.X - midSide), proposedContentOffset.Y);
            }
            else
            {
                var closest =
                    layoutAttributes.OrderBy(x => Math.Abs(x.Center.Y - proposedContentOffsetCenterOrigin))
                        .FirstOrDefault() ?? new UICollectionViewLayoutAttributes();
                targetContentOffset = new CGPoint(proposedContentOffset.X, Math.Floor(closest.Center.Y - midSide));
            }

            return targetContentOffset;
        }

        private void Initialize()
        {
            SideItemScale = 0.6f;
            SideItemAlpha = 0.6f;
            SideItemShift = 0f;
            _spacingMode = new UPCarouselFlowLayoutSpacingModeState(UPCarouselFlowLayoutSpacingMode.Fixed, 40f);
        }
    }
}