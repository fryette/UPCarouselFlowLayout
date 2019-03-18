using System;
using CoreGraphics;
using Foundation;
using UIKit;
using UPCarouselFlowLayout.Sample.Cells;

namespace UPCarouselFlowLayout.Sample
{
    internal class CustomCollectionViewSource : UICollectionViewSource
    {
        public CustomCollectionViewSource(Character[] items)
        {
            Items = items;
        }

        public Character[] Items { get; }
        private UICollectionView _collectionView;
        public event EventHandler<int> SelectionChanged;

        public override nint NumberOfSections(UICollectionView collectionView)
        {
            return 1;
        }

        public override nint GetItemsCount(UICollectionView collectionView, nint section)
        {
            if (_collectionView == null)
            {
                _collectionView = collectionView;
            }

            return Items.Length;
        }

        public override UICollectionViewCell GetCell(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var cell = (CircleCollectionViewCell)collectionView.DequeueReusableCell(CircleCollectionViewCell.Key,
                indexPath);
            var character = Items[indexPath.Row];
            cell.ImageView.Image = UIImage.FromBundle(character.ImageName);

            var scrollPosition = UIDevice.CurrentDevice.Orientation.IsPortrait() ? UICollectionViewScrollPosition.CenteredHorizontally : UICollectionViewScrollPosition.CenteredVertically;

            if (cell.GestureRecognizers != null)
            {
                foreach (var gr in cell.GestureRecognizers)
                {
                    cell.RemoveGestureRecognizer(gr);
                }
            }

            cell.AddGestureRecognizer(new UITapGestureRecognizer(() => ScrollToItem(collectionView, indexPath)));

            return cell;
        }

        private void ScrollToItem(UICollectionView collectionView, NSIndexPath indexPath)
        {
            var scrollPosition = UIDevice.CurrentDevice.Orientation.IsPortrait() ? UICollectionViewScrollPosition.CenteredHorizontally : UICollectionViewScrollPosition.CenteredVertically;

            collectionView.ScrollToItem(indexPath, scrollPosition, true);
            SelectionChanged?.Invoke(this, indexPath.Row);
        }

        public override void DecelerationEnded(UIScrollView scrollView)
        {
            var layout = (UPCarouselFlowLayout)_collectionView.CollectionViewLayout;

            var pageSide = layout.ScrollDirection == UICollectionViewScrollDirection.Horizontal
                ? GetPageSize().Width
                : GetPageSize().Height;

            var offset = layout.ScrollDirection == UICollectionViewScrollDirection.Horizontal
                ? scrollView.ContentOffset.X
                : scrollView.ContentOffset.Y;

            SelectionChanged?.Invoke(this, (int)Math.Floor((offset - pageSide / 2) / pageSide) + 1);
        }

        private CGSize GetPageSize()
        {
            var layout = (UPCarouselFlowLayout)_collectionView.CollectionViewLayout;

            var pageSize = layout.ItemSize;

            if (layout.ScrollDirection == UICollectionViewScrollDirection.Horizontal)
            {
                pageSize.Width += layout.MinimumLineSpacing;
            }
            else
            {
                pageSize.Height += layout.MinimumLineSpacing;
            }

            return pageSize;
        }
    }
}