using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using AssetsLibrary;
using Foundation;
using System.Diagnostics;

namespace iostest
{
	public class Photos
	{
		static SynchronizerDelegate sync;
		public static ALAssetsLibrary Library { get; private set; }

		List<string> _results;

		public Photos (SynchronizerDelegate sync)
		{
			Photos.sync = sync;
			Photos.Library = new ALAssetsLibrary ();
		}

		public void ImageSearch ()
		{
			Task.Run (() => DoSearch());
		}

		void DoSearch ()
		{
			try {
				_results = new List<string>();

				Library = new ALAssetsLibrary();
				Library.Enumerate(ALAssetsGroupType.SavedPhotos,
					GroupsEnumeration,
					GroupsEnumerationFailure
				);
			} catch (Exception ex) {
				Debug.WriteLine (ex.ToString());
				if (sync != null)
					sync (null);
			}
		}

		private void GroupsEnumeration(ALAssetsGroup group, ref bool stop)
		{
			if (group != null) {
				stop = false;

				group.SetAssetsFilter (ALAssetsFilter.AllPhotos);
				group.Enumerate (this.AssetsEnumeration);

				sync (_results);
			}
		}

		private void AssetsEnumeration(ALAsset asset, nint index, ref bool stop)
		{
			if (asset != null) {
				stop = false;

				_results.Add (asset.DefaultRepresentation.Url.ToString ());
			}
		}

		private void  GroupsEnumerationFailure(NSError error)
		{
			if (error != null)
			{
			}
		}

	}
}

