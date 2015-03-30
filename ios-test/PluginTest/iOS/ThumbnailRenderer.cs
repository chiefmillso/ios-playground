using Xamarin.Forms;
using PluginTest.iOS;
using Xamarin.Forms.Platform.iOS;
using PluginTest;
using UIKit;
using Foundation;
using System.Collections.Generic;
using System;
using System.Drawing;
using SDWebImage;
using System.Collections;
using System.Linq;

[assembly: ExportRenderer(typeof(ThumbnailListView), typeof(ThumbnailRenderer))]

namespace PluginTest.iOS
{
	public class ThumbnailRenderer : ViewRenderer<ThumbnailListView, UICollectionView>
	{
		static readonly NSString cellId = new NSString ("ImageCell");

		protected override void OnElementChanged (ElementChangedEventArgs<ThumbnailListView> e)
		{
			if (e.OldElement != null) {

			}
			if (e.NewElement != null) {
				var listView = e.NewElement;
				if (base.Control == null) {
					var flowLayout = new UICollectionViewFlowLayout ();
					var view = new UICollectionView (Frame, flowLayout);
					view.ContentInset = new UIEdgeInsets (10, 10, 10, 10);

					base.SetNativeControl (view);
					base.Control.Source = new ListViewDataSource (listView, base.Control);
				}
			}

			base.OnElementChanged (e);
		}

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			Control.ReloadData ();
		}

		public class ListViewDataSource : UICollectionViewSource
		{
			ThumbnailListView _list;
			UICollectionView _view;

			public ListViewDataSource (ThumbnailListView list, UICollectionView view)
			{
				_list = list;
				_view = view;
			}

			public override nint GetItemsCount (UICollectionView collectionView, nint section)
			{
				return 0;
			}

			public override UICollectionViewCell GetCell (UICollectionView collectionView, Foundation.NSIndexPath indexPath)
			{
				var imageCell = (ImageCell) collectionView.DequeueReusableCell (cellId, indexPath);
				imageCell.UpdateCell (null);
				return imageCell;
			}

		}
	}

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

		public void UpdateCell (IImageAsset asset)
		{
			var imageAsset = asset as ImageAsset;
			if (imageAsset != null)
				imageView.Image = imageAsset.NativeImage;
		}

		public void UpdatedImage (Uri uri)
		{
			imageView.SetImage (new NSUrl (uri.ToString ()));
		}
	}
}

