
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
		private LinearLayout noInternetLayout;
		private SwipeRefreshLayout refresher;
		private Button noInternetButton;
		private ViewGroup root;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override async void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);

			if (MainActivity.isOnline)
			{
				refresher.Refreshing = true;
				await GetStartNewsFeed();
			}
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_news, null);

			noInternetLayout = root.FindViewById<LinearLayout>(Resource.Id.noInternetContent);

			recyclerView = root.FindViewById<RecyclerView>(Resource.Id.recyclerViewNews);
			recyclerView.HasFixedSize = true;

			refresher = root.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
			refresher.Refresh += HandleRefresh;

			var layoutManager = new LinearLayoutManager(Activity);
			var onScrollListener = new XamarinRecyclerViewOnScrollListenerNews(layoutManager);
			onScrollListener.LoadMoreEvent += LoadMore;
			recyclerView.AddOnScrollListener(onScrollListener);
			recyclerView.SetLayoutManager(layoutManager);

			if(NewsCache.IsExist())GetCachedNews();

			newsAdapter = new NewsAdapter(newsFeed);
			recyclerView.SetAdapter(newsAdapter);

			if (!NewsCache.IsExist() && !MainActivity.isOnline)
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
			try
			{
				newsOffset = 20;
				var data = await DataLoader.GetNewsData();
				newsFeed.Clear();
				newsFeed.AddRange(data);
				newsAdapter.NotifyDataSetChanged();
				newsAdapter.ItemClick += NewsItemClick;
				refresher.Refreshing = false;
				if (MainActivity.isOnline && noInternetLayout.Visibility == ViewStates.Visible)
				{
					noInternetLayout.Visibility = ViewStates.Gone;
				}
			}
			catch (System.Net.Http.HttpRequestException)
			{
				refresher.Refreshing = false;
				ShowNoInternetNofitication(root);
			}
		}

		/// <summary>
		/// Get news feed when scrolling
		/// </summary>
		/// <returns>Array with new news</returns>
		public async Task GetAdditionalNewsFeed()
		{
			var data = await DataLoader.GetNewsData(newsOffset);
			newsFeed.AddRange(data);
			newsAdapter.NotifyItemInserted(newsFeed.Count);
			newsOffset += 20;
		}

		/// <summary>
		/// Get news from cache
		/// </summary>
		public void GetCachedNews()
		{
			JObject json = JObject.Parse(NewsCache.ReadNewsData());
			newsFeed = DataLoader.ParseNewsData(json).ToList();
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
				if (MainActivity.isOnline)
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
			if (MainActivity.isOnline)
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
			Console.WriteLine("Clicked");
		}
	}
}
