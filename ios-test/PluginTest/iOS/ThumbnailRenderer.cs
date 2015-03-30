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
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Collections.Specialized;

[assembly: ExportRenderer(typeof(ThumbnailListView), typeof(ThumbnailRenderer))]

namespace PluginTest.iOS
{
	public class ThumbnailRenderer : ViewRenderer<ThumbnailListView, UICollectionView>
	{
		static readonly NSString cellId = new NSString ("ImageCell");

		readonly ListViewDataSource source;

		public ThumbnailRenderer ()
		{
			source = new ListViewDataSource ();
		}

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
					CollectionView.Source = source;			
					CollectionView.RegisterClassForCell (typeof(ImageCell), cellId);
					Assets = listView.Assets;
				}
			}

			base.OnElementChanged (e);
		}

		public ObservableCollection<IImageAsset> Assets
		{
			get { return source.Assets; }
			set {
				try {
					if (source.Assets != null) {
						source.Assets.CollectionChanged -= VisitsOnCollectionChanged;
					}
				} catch (Exception e) {
					Debug.WriteLine (e.Message);
				}
				source.Assets = value;
				if (source.Assets != null) {
					source.Assets.CollectionChanged += VisitsOnCollectionChanged;
				}
				CollectionView.ReloadData ();
			}
		}

		UICollectionView CollectionView {
			get {
				return Control;
			}
		}

		void VisitsOnCollectionChanged (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Remove)
			{
//				BeginInvokeOnMainThread (() => {
					if (e.NewItems != null) {
						var indexPaths = new List<NSIndexPath> ();
						for (int i = 0; i < e.NewItems.Count; i++)
							indexPaths.Add (NSIndexPath.FromRowSection (e.NewStartingIndex + i, 0));
						CollectionView.InsertItems (indexPaths.ToArray ());
					}

					if (e.OldItems != null) {
						var indexPaths = new List<NSIndexPath> ();
						for (int i = 0; i < e.OldItems.Count; i++)
							indexPaths.Add (NSIndexPath.FromRowSection (e.OldStartingIndex + i, 0));
						CollectionView.DeleteItems (indexPaths.ToArray ());
					}
//				);
			}
		}

		public class ListViewDataSource : UICollectionViewSource
		{
			public ObservableCollection<IImageAsset> Assets { get; set; }

			public override nint GetItemsCount (UICollectionView collectionView, nint section)
			{
				if (Assets == null)
					return 0;
				return Assets.Count;
			}

			public override UICollectionViewCell GetCell (UICollectionView collectionView, Foundation.NSIndexPath indexPath)
			{
				var imageCell = (ImageCell) collectionView.DequeueReusableCell (cellId, indexPath);
				var asset = Assets[indexPath.Row];
				imageCell.UpdateCell (asset);
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

