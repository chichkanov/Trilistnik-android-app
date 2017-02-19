
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
		private const string apiurl = "https://api.vk.com/method/wall.get?v=5.50&count=20&owner_id=-33374477&filter=owner";
		private const string apiurlAdd = "https://api.vk.com/method/wall.get?v=5.50&count=20&owner_id=-33374477&filter=owner&offset=";

		private RecyclerView recyclerView;
		private NewsAdapter newsAdapter;
		private List<Post> newsFeed = new List<Post>();
		private NewsCache newsCache;
		private LinearLayout noInternetLayout;
		private SwipeRefreshLayout refresher;
		private Button noInternetButton;
		private ViewGroup root;
		private InternetReceiver internetReceiver;
		private bool isLoading = false;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			internetReceiver = new InternetReceiver();
			newsCache = new NewsCache();
			internetReceiver.InternetConnectionLost += (sender, e) =>
			{
				if (isLoading)
				{
					Toast.MakeText(MainActivity.context, "Для загрузки новостей ребуется интернет соединение", ToastLength.Short).Show();
					isLoading = false;
				}
			};
		}

		public async override void OnActivityCreated(Bundle savedInstanceState)
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

			if(newsCache.IsExist())GetCachedNews();

			if (!newsCache.IsExist() && !MainActivity.isOnline(MainActivity.context))
			{
				ShowNoInternetNofitication(root);
			}
			return root;
		}

		public override void OnResume()
		{
			base.OnResume();
			IntentFilter intentFilter = new IntentFilter();
			intentFilter.AddAction("android.net.conn.CONNECTIVITY_CHANGE");
			Activity.RegisterReceiver(internetReceiver, intentFilter);
		}

		public override void OnStop()
		{
			base.OnResume();
			Activity.UnregisterReceiver(internetReceiver);
		}

		public async Task GetStartNewsFeed()
		{
			try
			{
				isLoading = true;
				using (var w = new WebClient())
				{
					newsOffset = 20;
					w.Encoding = Encoding.UTF8;
					string resp = await w.DownloadStringTaskAsync(apiurl);
					newsFeed = GetNews(resp, true).ToList();
					newsAdapter = new NewsAdapter(newsFeed);
					newsAdapter.ItemClick += NewsItemClick;
					if (MainActivity.isOnline(MainActivity.context) && noInternetLayout.Visibility == ViewStates.Visible)
					{
						noInternetLayout.Visibility = ViewStates.Gone;
					}
					recyclerView.SetAdapter(newsAdapter);
					refresher.Refreshing = false;
					isLoading = false;
				}
			}
			catch (System.Net.WebException)
			{
				refresher.Refreshing = false;
			}
		}

		public async Task GetAdditionalNewsFeed(int offset)
		{
			try
			{
				isLoading = true;
				using (var w = new WebClient())
				{
					newsOffset += 20;
					w.Encoding = Encoding.UTF8;
					string resp = await w.DownloadStringTaskAsync(apiurlAdd + offset);
					newsFeed.AddRange(GetNews(resp));
					newsAdapter.NotifyItemInserted(newsFeed.Count);
					isLoading = false;
				}
			}
			catch (System.Net.WebException)
			{
			}
		}

		public void GetCachedNews()
		{
			JObject json = JObject.Parse(newsCache.ReadNewsData());
			newsFeed = ParsePostsInJson(json).ToList();
			Log.Debug("dasds", newsFeed[0].Text);
			newsAdapter = new NewsAdapter(newsFeed);
			recyclerView.SetAdapter(newsAdapter);
		}

		/// <summary>
		/// Get news feed
		/// </summary>
		/// <returns>The news</returns>
		/// <param name="resp">Resp url</param>
		/// <param name="isStartUpdate">Cache news<c>cache after app loading</c>do not cache if it is lazy laod</param>
		private IEnumerable<Post> GetNews(string resp, bool isStartUpdate = false)
		{
			JObject json = JObject.Parse(resp);

			if (isStartUpdate && MainActivity.isOnline(MainActivity.context))
			{
				newsCache = new NewsCache();
				newsCache.CreateNewsData(json.ToString());
			}

			JArray posts = (JArray)json["response"]["items"];

			return ParsePostsInJson(json);
		}

		private IEnumerable<Post> ParsePostsInJson(JObject json)
		{
			JArray posts = (JArray)json["response"]["items"];

			var newsFeedLocal = from post in posts
								where post["text"].ToString() != String.Empty
								select new Post
								{
									Text = post["text"].ToString(),
									Date = post["date"].ToString(),
									Img = post["attachments"]?[0]?["photo"]?["photo_604"].ToString()
								};
			return newsFeedLocal;
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
			await GetAdditionalNewsFeed(newsOffset);
		}

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
