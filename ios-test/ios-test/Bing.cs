using System;
using UIKit;
using Foundation;
using System.Drawing;
using System.Net;
using System.Threading.Tasks;
using System.Json;
using System.Linq;
using System.Collections;

namespace iostest
{

	public class Bing
	{
		const string AZURE_KEY = "TqNaxOs+ilo74tYT4hWVtyakUxQ0utIdR6p/wB5WOO4=";

		static SynchronizerDelegate sync;

		public Bing (SynchronizerDelegate sync)
		{
			Bing.sync = sync;
		}

		public void ImageSearch ()
		{
			Task.Run (() => DoSearch());
		}

		void DoSearch ()
		{
			string uri = "https://api.datamarket.azure.com/Data.ashx/Bing/Search/v1/Image?Query=%27xamarin%27&$top=50&$format=Json";

			var httpReq = (HttpWebRequest) WebRequest.Create (new Uri (uri));

			httpReq.Credentials = new NetworkCredential (AZURE_KEY, AZURE_KEY);

			try {
				using (HttpWebResponse httpRes = (HttpWebResponse)httpReq.GetResponse ()) {

					var response = httpRes.GetResponseStream ();
					var json = (JsonObject) JsonValue.Load (response);

					var results = ((json["d"]["results"] as JsonArray) as IEnumerable)
						.OfType<JsonObject>()
						.Select(x => x["Thumbnail"]["MediaUrl"].ToString())
						.ToList();

					if (sync != null)
						sync (results);
				}
			} catch (Exception) {
				if (sync != null)
					sync (null);
			}
		}

	}
	
}
