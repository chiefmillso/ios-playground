using System;
using Xamarin.Forms;

namespace PluginTest
{
	public class ImageCell : ViewCell
	{
		public ImageCell ()
		{
			var image = new Image ();
			image.SetBinding(Image.SourceProperty, new Binding("Source"));
			image.WidthRequest = image.HeightRequest = 40;
			View = image;
		}
	}
}

