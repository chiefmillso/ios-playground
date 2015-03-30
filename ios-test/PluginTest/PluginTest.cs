using System;
using System.Collections.ObjectModel;
using System.Reactive;
using Xamarin.Forms;
using System.Reactive.Linq;
using System.Diagnostics;
using System.Threading.Tasks;


namespace PluginTest
{
	public class App : Application
	{
		private IDisposable subscription;

		public App ()
		{
			var context = System.Threading.SynchronizationContext.Current;
			var items = new ObservableCollection<IImageAsset> ();
			var provider = DependencyService.Get<IAssetsProvider> ();
			DependencyService.Get<IThreadMessage> ().Log ("Initialise");

			var dataTemplate = new DataTemplate (typeof(ImageCell));
			var listView = new ThumbnailListView () {
				Assets = items
			};
			listView.ItemTemplate = dataTemplate;

			MainPage = new ContentPage {
				Content = listView
			};

			var result = provider.Query (AssetType.Photos);
			subscription = result.Assets
				.ObserveOn (context)
				.OfType<IImageAsset>()
				.Subscribe ((asset) => {
				DependencyService.Get<IThreadMessage> ().Log ("Received Message");
				items.Add (asset);
			});
			
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps

			if (subscription != null)
				subscription.Dispose ();
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

