
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Trilistnik
{
	[Activity(Label = "NewsPost")]
	public class NewsPost : Activity
	{
		public static readonly string VIEW_NAME_NEWS_TEXT = "detail:main:text";
		public static readonly string VIEW_NAME_NEWS_IMG = "detail:main:img";

		private TextView postText;
		private ImageView postImg;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.NewsPost);

			postText = FindViewById<TextView>(Resource.Id.newsPostText);
			postText.TransitionName = VIEW_NAME_NEWS_TEXT;
			postText.Text = Intent.GetStringExtra("newsText");

			postImg = FindViewById<ImageView>(Resource.Id.newsImage);
			postText.TransitionName = VIEW_NAME_NEWS_IMG;
		}
	}
}
