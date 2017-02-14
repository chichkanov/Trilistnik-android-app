
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

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			ViewGroup root;

			root = (ViewGroup)inflater.Inflate(Resource.Layout.newsfragment, null);
			recyclerView = root.FindViewById<RecyclerView>(Resource.Id.recyclerViewNews);
			if (recyclerView != null)
			{
				recyclerView.HasFixedSize = true;
				var layoutManager = new LinearLayoutManager(Activity);
				var onScrollListener = new XamarinRecyclerViewOnScrollListener(layoutManager);
				onScrollListener.LoadMoreEvent += LoadMore;
				recyclerView.AddOnScrollListener(onScrollListener);
				recyclerView.SetLayoutManager(layoutManager);
				GetCachedNews();
			}

			return root;
		}


		public async Task GetStartNewsFeed(SwipeRefreshLayout refresher)
		{

			using (var w = new WebClient())
			{
				newsOffset = 20;
				w.Encoding = Encoding.UTF8;
				string resp = await w.DownloadStringTaskAsync(apiurl);
				newsFeed = GetNews(resp, true).ToList();
				newsAdapter = new NewsAdapter(newsFeed);
				recyclerView.SetAdapter(newsAdapter);
				refresher.Refreshing = false;
			}
		}

		public async Task GetAdditionalNewsFeed(int offset)
		{
			using (var w = new WebClient())
			{
				newsOffset += 20;
				w.Encoding = Encoding.UTF8;
				string resp = await w.DownloadStringTaskAsync(apiurlAdd + offset);
				newsFeed.AddRange(GetNews(resp));
				newsAdapter.NotifyItemInserted(newsFeed.Count);
			}
		}

		public void GetCachedNews()
		{
			NewsCache newsCache = new NewsCache();
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
				NewsCache newsCache = new NewsCache();
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
		/// Load more event
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">Args</param>
		async void LoadMore(object sender, EventArgs e)
		{
			await GetAdditionalNewsFeed(newsOffset);
		}
	}
}
