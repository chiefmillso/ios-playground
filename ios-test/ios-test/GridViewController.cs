using System;
using UIKit;
using Foundation;
using System.Collections.Generic;
using System.Drawing;

namespace iostest
{
	public class GridViewController : UICollectionViewController
	{
		static readonly NSString cellId = new NSString ("ImageCell");

		List<string> imageUrls = new List<string>();
		//Bing bing;
		Photos photos;

		public GridViewController (UICollectionViewLayout layout) : base (layout)
		{
			imageUrls = new List<string> ();

			photos = new Photos ((results) => InvokeOnMainThread (delegate {
				imageUrls = results;
				CollectionView.ReloadData ();
			}));

			photos.ImageSearch ();

			CollectionView.ContentInset = new UIEdgeInsets (10, 10, 10, 10);
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			CollectionView.RegisterClassForCell (typeof(ImageCell), cellId);
		}

		public override nint GetItemsCount (UICollectionView collectionView, nint section)
		{
			return imageUrls.Count;
		}

		public override UICollectionViewCell GetCell (UICollectionView collectionView, NSIndexPath indexPath)
		{
			var imageCell = (ImageCell)collectionView.DequeueReusableCell (cellId, indexPath);
			string imageUrl = imageUrls [indexPath.Row].Replace ("\"", "");
			imageCell.UpdateCell (new Uri (imageUrl));
			return imageCell;
		}
	}

}

