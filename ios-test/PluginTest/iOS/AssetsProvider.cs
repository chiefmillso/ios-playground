using System;
using Xamarin.Forms;
using PluginTest.iOS;
using AssetsLibrary;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using UIKit;

[assembly: Dependency(typeof(AssetsProvider))]
[assembly: Dependency(typeof(ThreadMessage))]

namespace PluginTest.iOS
{
	public class ThreadMessage : IThreadMessage
	{
		public void Log (string message)
		{
			Debug.WriteLine (string.Format("ThreadId:{0}:{1}", Thread.CurrentThread.ManagedThreadId, message));
		}
	}

	public class AssetsProvider : IAssetsProvider
	{
		#region IAssetsProvider implementation

		public IAssetsEnumeration Query (AssetType filterType)
		{
			return new AssetsQuery (filterType);
		}
			
		#endregion
	
		internal class AssetsQuery : IAssetsEnumeration, IDisposable
		{
			#region IDisposable implementation

			public void Dispose ()
			{
				if (_subject != null)
					_subject.Dispose ();
				if (_filter != null)
					_filter.Dispose ();
				if (_library != null)
					_library.Dispose ();
			}

			#endregion

			ALAssetsLibrary _library;

			private readonly Subject<IAsset> _subject = new Subject<IAsset>();

			public AssetType FilterType {
				get;
				private set;
			}

			private ALAssetsFilter _filter;

			public IObservable<IAsset> Assets { get { return _subject.AsObservable (); } }

			public AssetsQuery (AssetType filterType)
			{
				_library = new ALAssetsLibrary();
				FilterType = filterType;
				_filter = ALAssetsFilter.AllPhotos;

				Task.Run(() => {
					_library.Enumerate(ALAssetsGroupType.SavedPhotos, enumerate, failureBlock);
				});
			}

			void enumerate (ALAssetsGroup group, ref bool stop)
			{
				if (group != null) {
					stop = false;

					group.SetAssetsFilter (_filter);
					group.Enumerate (enumerate);
				}
			}

			void enumerate (ALAsset asset, nint index, ref bool stop)
			{
				if (asset != null) {
					stop = false;
					var wrapper = new ImageAsset(asset, index);
					DependencyService.Get<IThreadMessage>().Log("Invoked");
					_subject.OnNext (wrapper);
				}
			}

			void failureBlock (Foundation.NSError obj)
			{
				// todo: handler error
			}
		}
	}

	public class ImageAsset : AssetBase, IImageAsset
	{
		public ImageAsset (ALAsset asset, nint index) : base(asset, index)
		{
			Type = AssetType.Photos;
		}

		public ImageSource Source {
			get {
				return ImageSource.FromStream (() => {
					var cImage = Asset.DefaultRepresentation.GetImage();
					var image = UIImage.FromImage(cImage);
					return image.AsPNG().AsStream();
				});
			}
		}

		internal UIImage NativeImage {
			get {
				var cImage = Asset.DefaultRepresentation.GetImage();
				return UIImage.FromImage(cImage);
			}
		}
	}

	public abstract class AssetBase : IAsset {
		#region IAsset implementation

		public AssetType Type {
			get;
			protected set;
		}

		public string Url {
			get;
			private set;
		}

		protected ALAsset Asset { get; private set; }

		#endregion

		protected AssetBase (ALAsset asset, nint index)
		{
			Asset = asset;
			Url = asset.DefaultRepresentation.Url.ToString ();
			Type = AssetType.All;
		}
	}
}

