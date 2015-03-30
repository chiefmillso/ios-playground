using System;
using UIKit;
using Foundation;
using System.Drawing;
using System.Collections.Generic;
using SDWebImage;
using AssetsLibrary;

namespace iostest
{
	public delegate void SynchronizerDelegate (List<string> results);

	public class ImageCell : UICollectionViewCell
	{
		readonly UIImageView imageView;

		[Export ("initWithFrame:")]
		public ImageCell (System.Drawing.RectangleF frame) : base (frame)
		{
			imageView = new UIImageView (new RectangleF (0, 0, 50, 50)); 
			imageView.ContentMode = UIViewContentMode.ScaleAspectFit;
			ContentView.AddSubview (imageView);
		}

		public void UpdateCell (Uri uri)
		{
			if (uri.Scheme == "assets-library") {
				var library = Photos.Library;
				var nsUrl = new NSUrl (uri.ToString ());
				library.AssetForUrl (nsUrl, (asset) => {
					var image = asset.DefaultRepresentation.GetImage();
					imageView.Image = UIImage.FromImage(image);
				}, (error) => {
				});
			} else {
				imageView.SetImage (new NSUrl (uri.ToString ()));
			}
		}

		public void UpdatedImage (Uri uri)
		{
			imageView.SetImage (new NSUrl (uri.ToString ()));
		}
	}
}
