﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Webkit;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Android.Support.Design.Widget;

namespace Trilistnik
{
	public class PayFragment : Android.Support.V4.App.Fragment
	{
		private WebView webView;
		private View loadingSpinner;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_pay, null);

			webView = root.FindViewById<WebView>(Resource.Id.webViewPay);
			loadingSpinner = root.FindViewById(Resource.Id.loading_spinner);

			WebSettings webSettings = webView.Settings;
			webSettings.SetAppCacheMaxSize(1024 * 1024 * 8);
			webSettings.SetAppCachePath("data/data/com.chichkanov.trilistnik/cache");
			webSettings.SetAppCacheEnabled(true);
			webSettings.CacheMode = CacheModes.CacheElseNetwork;
			webSettings.JavaScriptEnabled = true;
			webView.SetWebViewClient(new MyWebViewClient(loadingSpinner, webView));

			if (MainActivity.isOnline)
			{
				webView.LoadUrl("https://pay.hse.ru/moscow/prg");
			}
			else 
			{
				root = (ViewGroup)inflater.Inflate(Resource.Layout.fragmentNoInternet, null);
			}

			return root;
		}
	}
}
