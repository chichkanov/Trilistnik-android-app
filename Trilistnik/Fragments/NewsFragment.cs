﻿
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Trilistnik
{
	public class NewsFragment : Android.Support.V4.App.Fragment
	{
		private int newsOffset = 20;
		private RecyclerView recyclerView;
		private NewsAdapter newsAdapter;
		private List<Post> newsFeed = new List<Post>();
		private NewsCache newsCache;
		private LinearLayout noInternetLayout;
		private SwipeRefreshLayout refresher;
		private Button noInternetButton;
		private ViewGroup root;
		private bool isLoading = false;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			newsCache = new NewsCache();
		}

		public override async void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			if (MainActivity.isOnline(MainActivity.context))
			{
				refresher.Refreshing = true;
				await GetStartNewsFeed();
			}
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			root = (ViewGroup)inflater.Inflate(Resource.Layout.newsfragment, null);

			noInternetLayout = root.FindViewById<LinearLayout>(Resource.Id.noInternetContent);

			recyclerView = root.FindViewById<RecyclerView>(Resource.Id.recyclerViewNews);
			recyclerView.HasFixedSize = true;

			refresher = root.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
			refresher.Refresh += HandleRefresh;

			var layoutManager = new LinearLayoutManager(Activity);
			var onScrollListener = new XamarinRecyclerViewOnScrollListener(layoutManager);
			onScrollListener.LoadMoreEvent += LoadMore;
			recyclerView.AddOnScrollListener(onScrollListener);
			recyclerView.SetLayoutManager(layoutManager);

			if(NewsCache.IsExist())GetCachedNews();

			if (!NewsCache.IsExist() && !MainActivity.isOnline(MainActivity.context))
			{
				ShowNoInternetNofitication(root);
			}


			return root;
		}

		/// <summary>
		/// Get news feed when app starts
		/// </summary>
		/// <returns>Array with start news feed</returns>
		public async Task GetStartNewsFeed()
		{
			newsOffset = 20;
			isLoading = true;
			var data = await JsonDataLoader.GetData();
			newsFeed = data.ToList();
			newsAdapter = new NewsAdapter(newsFeed);
			newsAdapter.ItemClick += NewsItemClick;
			recyclerView.SetAdapter(newsAdapter);
			refresher.Refreshing = false;
			isLoading = false;
			if (MainActivity.isOnline(MainActivity.context) && noInternetLayout.Visibility == ViewStates.Visible)
			{
				noInternetLayout.Visibility = ViewStates.Gone;
			}
		}

		/// <summary>
		/// Get news feed when scrolling
		/// </summary>
		/// <returns>Array with new news</returns>
		public async Task GetAdditionalNewsFeed()
		{
			isLoading = true;
			var data = await JsonDataLoader.GetData(newsOffset);
			newsFeed.AddRange(data);
			newsAdapter.NotifyItemInserted(newsFeed.Count);
			isLoading = false;
			newsOffset += 20;
		}

		/// <summary>
		/// Get news from cache
		/// </summary>
		public void GetCachedNews()
		{
			JObject json = JObject.Parse(NewsCache.ReadNewsData());
			newsFeed = JsonDataLoader.ParseData(json).ToList();
			newsAdapter = new NewsAdapter(newsFeed);
			recyclerView.SetAdapter(newsAdapter);
		}

		/// <summary>
		/// Show ui according to internet connection
		/// </summary>
		/// <param name="root">Root viewGroup</param>
		private void ShowNoInternetNofitication(ViewGroup root)
		{
			noInternetLayout.Visibility = ViewStates.Visible;
			for (int i = 0; i < noInternetLayout.ChildCount; i++)
			{
				noInternetLayout.GetChildAt(i).Visibility = ViewStates.Visible;
			}
			noInternetButton = root.FindViewById<Button>(Resource.Id.buttonRepeatConnection);
			noInternetButton.Click += async (sender, e) =>
			{
				if (MainActivity.isOnline(MainActivity.context))
				{
					await GetStartNewsFeed();
				}
			};
		}

		/// <summary>
		/// Load more event
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">Args</param>
		async void LoadMore(object sender, EventArgs e)
		{
			await GetAdditionalNewsFeed();
		}

		/// <summary>
		/// Handle swipe refresher refresh
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		async void HandleRefresh(object sender, EventArgs e)
		{
			if (MainActivity.isOnline(MainActivity.context))
			{
				await GetStartNewsFeed();
			}
			else
			{
				Toast.MakeText(MainActivity.context, "Для загрузки новостей ребуется интернет соединение", ToastLength.Short).Show();
				refresher.Refreshing = false;
			}
		}

		private void NewsItemClick(object sender, int position)
		{
			Log.Debug("dasdasd", position.ToString());
			var newsPostIntent = new Intent(MainActivity.context, typeof(NewsPost));
			newsPostIntent.PutExtra("newsText", root.FindViewById<TextView>(Resource.Id.newsText).Text);
			ActivityOptions activityOptions = ActivityOptions.MakeSceneTransitionAnimation(
				Activity,
				new Pair(root.FindViewById(Resource.Id.newsText), NewsPost.VIEW_NAME_NEWS_TEXT),
				new Pair(root.FindViewById(Resource.Id.newsImg), NewsPost.VIEW_NAME_NEWS_IMG)
			);
			StartActivity(newsPostIntent, activityOptions.ToBundle());
		}
	}
}
