using System;
using Xamarin.Forms;

namespace PluginTest
{
	public interface IAssetsProvider
	{
		IAssetsEnumeration Query(AssetType filterType);
	}

	public enum AssetType
	{
		All,
		Photos,
		Videos
	}

	public interface IThreadMessage
	{
		void Log(string message);
	}

	public interface IAssetsEnumeration
	{
		AssetType FilterType { get; }
		IObservable<IAsset> Assets { get; }
	}

	public interface IAsset {
		AssetType Type { get; }
		string Url { get; }
	}

	public interface IImageAsset : IAsset {
		ImageSource Source { get; }
	}

	public class Asset : IAsset {
		
		public AssetType Type { get; set;}
		public string Url { get; set; }

		public override string ToString ()
		{
			return string.Format ("[Url={1}]", Type, Url);
		}
	}
}

