
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Webkit;

namespace Trilistnik
{
	public class FixFragment : Android.Support.V4.App.Fragment
	{

		private WebView webView;
		public override void OnCreate(Bundle savedInstanceState)
		{
			
			base.OnCreate(savedInstanceState);

			// Create your fragment here
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.fixfragment, null);
			if (MainActivity.isOnline(MainActivity.context))
			{
				webView = root.FindViewById<WebView>(Resource.Id.webViewFix);
				WebSettings webSettings = webView.Settings;
				webSettings.SetAppCacheMaxSize(1024 * 1024 * 8);
				webSettings.SetAppCachePath("data/data/com.chichkanov.trilistnik/cache");
				webSettings.SetAppCacheEnabled(true);
				webSettings.CacheMode = CacheModes.CacheElseNetwork;
				webSettings.JavaScriptEnabled = true;
				webView.SetWebViewClient(new WebViewClient());
				webView.LoadUrl("https://docs.google.com/forms/d/e/1FAIpQLSf4vWU_COpT-YYVObBryKLCt_-E-MyUAbAnlQQICLh9yWcRiQ/viewform?c=0&w=1");

			}
			else {
				root = (ViewGroup)inflater.Inflate(Resource.Layout.fragmentNoInternet, null);
			}
			return root;
		}

	}
}
