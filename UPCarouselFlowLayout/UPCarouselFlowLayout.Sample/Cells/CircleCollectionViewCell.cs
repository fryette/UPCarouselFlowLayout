using System;

using Foundation;
using UIKit;

namespace UPCarouselFlowLayout.Sample.Cells
{
    public partial class CircleCollectionViewCell : UICollectionViewCell
    {
        public static readonly NSString Key = new NSString(nameof(CircleCollectionViewCell));
        public static readonly UINib Nib;

        static CircleCollectionViewCell()
        {
            Nib = UINib.FromName(Key, NSBundle.MainBundle);
        }

        protected CircleCollectionViewCell(IntPtr handle) : base(handle)
        {
        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            Layer.CornerRadius = Frame.Size.Width / 2;
            Layer.BorderWidth = 10;
            Layer.BorderColor = UIColor.FromRGBA(110f / 255f, 80f / 255f, 140f / 255f, 1f).CGColor;
        }

        public UIImageView ImageView => Image;
    }
}
