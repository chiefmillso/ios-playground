using System;
using Xamarin.Forms;
using System.Collections.ObjectModel;

namespace PluginTest
{
	public class ThumbnailListView : ListView
	{
		public ObservableCollection<IImageAsset> Assets { get { return this.ItemsSource as ObservableCollection<IImageAsset>; } set { this.ItemsSource = value; } }
	}
}