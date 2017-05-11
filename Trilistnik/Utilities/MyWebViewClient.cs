using System;
using Android.Views;
using Android.Webkit;
using Android.Widget;

namespace Trilistnik
{
	public class MyWebViewClient : WebViewClient
	{
		private View loadingSpinner;
		private WebView webView;

		public MyWebViewClient(View loadingSpinner, WebView webView)
		{
			this.loadingSpinner = loadingSpinner;
			this.webView = webView;
		}

		public override void OnPageStarted(WebView view, string url, Android.Graphics.Bitmap favicon)
		{
			base.OnPageStarted(view, url, favicon);
			loadingSpinner.Visibility = ViewStates.Visible;
			webView.Visibility = ViewStates.Invisible;
		}

		public override void OnPageFinished(WebView view, string url)
		{
			base.OnPageFinished(view, url);
			loadingSpinner.Visibility = ViewStates.Gone;
			webView.Visibility = ViewStates.Visible;
		}

		public override void OnReceivedSslError(WebView view, SslErrorHandler handler, Android.Net.Http.SslError error)
		{
			base.OnReceivedSslError(view, handler, error);
			Toast.MakeText(view.Context, "Упс...", ToastLength.Long).Show();
		}

	}
}
