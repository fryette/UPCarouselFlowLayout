// WARNING
//
// This file has been generated automatically by Visual Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;

namespace UPCarouselFlowLayout.Sample.Cells
{
	[Register ("CircleCollectionViewCell")]
	partial class CircleCollectionViewCell
	{
		[Outlet]
		UIKit.UIImageView Image { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Image != null) {
				Image.Dispose ();
				Image = null;
			}
		}
	}
}
