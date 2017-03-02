
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
	public class TransportFragment : Android.Support.V4.App.Fragment
	{
		private RecyclerView recyclerView;
		private TransportAdapter transportAdapter;
		private List<TrainInfo> transportFeed = new List<TrainInfo>();
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
				await GetStartTransportFeed();
			}
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			ViewGroup root = (ViewGroup)inflater.Inflate(Resource.Layout.transportfragment, null);

			noInternetLayout = root.FindViewById<LinearLayout>(Resource.Id.noInternetContentTransport);

			recyclerView = root.FindViewById<RecyclerView>(Resource.Id.recyclerViewTransport);
			recyclerView.HasFixedSize = true;

			refresher = root.FindViewById<SwipeRefreshLayout>(Resource.Id.refresherTransport);
			//refresher.Refresh += HandleRefresh;

			var layoutManager = new LinearLayoutManager(Activity);
			//var onScrollListener = new XamarinRecyclerViewOnScrollListenerNews(layoutManager);
			//onScrollListener.LoadMoreEvent += LoadMore;
			//recyclerView.AddOnScrollListener(onScrollListener);
			recyclerView.SetLayoutManager(layoutManager);


			return root;
		}

		/// <summary>
		/// Get news feed when app starts
		/// </summary>
		/// <returns>Array with start news feed</returns>
		public async Task GetStartTransportFeed()
		{
			try
			{
				var data = await JsonDataLoader.GetTransportData();
				transportFeed = data.ToList();
				transportAdapter = new TransportAdapter(transportFeed);
				//newsAdapter.ItemClick += NewsItemClick;
				recyclerView.SetAdapter(transportAdapter);
				refresher.Refreshing = false;
				if (MainActivity.isOnline && noInternetLayout.Visibility == ViewStates.Visible)
				{
					noInternetLayout.Visibility = ViewStates.Gone;
				}
			}
			catch (System.ArgumentNullException)
			{
				refresher.Refreshing = false;
				//ShowNoInternetNofitication(root);
			}
		}

	}
}
