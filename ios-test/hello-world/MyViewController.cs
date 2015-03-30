using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;
using System.Collections.Generic;

namespace helloworld
{
	partial class MyViewController : UIViewController
	{
		public MyViewController (IntPtr handle) : base (handle)
		{
			PhoneNumbers = new List<String> ();
		}

		public List<String> PhoneNumbers { get; set; }

		public override void PrepareForSegue (UIStoryboardSegue segue, NSObject sender)
		{
			base.PrepareForSegue (segue, sender);

			var controller = segue.DestinationViewController as CallHistoryViewController;
			if (controller != null) {
				controller.PhoneNumbers = PhoneNumbers;
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			string sourceNumber = "";

			TranslateButton.TouchUpInside += (sender, e) => {

				sourceNumber = PhonewordTranslator.ToNumber(PhoneNumberText.Text);

				// Dismiss the keyboard if text field was tapped
				PhoneNumberText.ResignFirstResponder();

				if (sourceNumber == "") {
					CallButton.SetTitle("Call", UIControlState.Normal);
					CallButton.Enabled = false;
				} else {
					CallButton.SetTitle("Call: "+ sourceNumber, UIControlState.Normal);
					CallButton.Enabled = true;
				}
			};

			CallButton.TouchUpInside += (sender, e) => {

				PhoneNumbers.Add (sourceNumber);

				var url = new NSUrl ("tel:" + sourceNumber);

				// Use URL handler with tel: prefix to invoke Apple's Phone app, 
				// otherwise show an alert dialog                

				if (!UIApplication.SharedApplication.OpenUrl (url)) {
					var av = new UIAlertView ("Not supported",
						"Scheme 'tel:' is not supported on this device",
						null,
						"OK",
						null);
					av.Show ();
				}

			};
		}
	}
}
