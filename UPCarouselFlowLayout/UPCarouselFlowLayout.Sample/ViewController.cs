using System;
using Foundation;
using UIKit;
using UPCarouselFlowLayout.Sample.Cells;

namespace UPCarouselFlowLayout.Sample
{
    public partial class ViewController : UIViewController
    {
        private Character[] _items;
        private UPCarouselFlowLayout _flowLayout;
        private int _selectedIndex;

        public ViewController(IntPtr handle) : base(handle)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupLayout();
            InitializeLabels(0);

            var source = new CustomCollectionViewSource(_items);
            source.SelectionChanged += OnSourceOnSelectionChanged;
            CollectionView.Source = source;
            CollectionView.RegisterNibForCell(CircleCollectionViewCell.Nib, CircleCollectionViewCell.Key);

            UIDevice.Notifications.ObserveOrientationDidChange(RotationDidChanged);
        }

        private void OnSourceOnSelectionChanged(object sender, int index)
        {
            _selectedIndex = index;
            InitializeLabels(index);
        }

        private void InitializeLabels(int index)
        {
            TitleLabel.Text = _items[index].Name;
            SecondaryTitleLabel.Text = _items[index].Movie;
        }

        private void SetupLayout()
        {
            _flowLayout = (UPCarouselFlowLayout)CollectionView.CollectionViewLayout;
            _flowLayout.SetSpacingMode(UPCarouselFlowLayoutSpacingMode.Overlap, 30);

            _items = new[]
            {
                new Character("wall-e", "Wall-E", "Wall-E"),
                new Character("nemo", "Nemo", "Finding Nemo"),
                new Character("ratatouille", "Remy", "Ratatouille"),
                new Character("buzz", "Buzz Lightyear", "Toy Story"),
                new Character("monsters", "Mike & Sullivan", "Monsters Inc."),
                new Character("brave", "Merida", "Brave")
            };
        }

        private void RotationDidChanged(object sender, NSNotificationEventArgs nsNotificationEventArgs)
        {
            var orientation = UIDevice.CurrentDevice.Orientation;

            if (orientation.IsFlat())
            {
                return;
            }

            var directional = orientation.IsPortrait()
                ? UICollectionViewScrollDirection.Horizontal
                : UICollectionViewScrollDirection.Vertical;
            _flowLayout.ScrollDirection = directional;

            if (_selectedIndex == 0)
            {
                return;
            }

            var indexPath = NSIndexPath.FromItemSection(_selectedIndex, 0);
            var scrollPosition = orientation.IsPortrait()
                ? UICollectionViewScrollPosition.CenteredHorizontally
                : UICollectionViewScrollPosition.CenteredVertically;
            CollectionView.ScrollToItem(indexPath, scrollPosition, false);
        }
    }
}