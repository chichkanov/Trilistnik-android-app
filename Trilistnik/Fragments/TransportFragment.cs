
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
		private Spinner fromSpinner, toSpinner;
		private string currentFromStation, currentToStation;

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
				await GetStartTransportFeed(currentFromStation, currentToStation, DateTime.Now.ToString("yyyy-MM-dd"));
			}
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			root = (ViewGroup)inflater.Inflate(Resource.Layout.transportfragment, null);

			noInternetLayout = root.FindViewById<LinearLayout>(Resource.Id.noInternetContentTransport);
			fromSpinner = root.FindViewById<Spinner>(Resource.Id.fromSpinner);
			toSpinner = root.FindViewById<Spinner>(Resource.Id.toSpinner);

			var adapterFrom = ArrayAdapter.CreateFromResource(MainActivity.context, Resource.Array.fromArray, Resource.Layout.spinner);
			adapterFrom.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

			var adapterTo = ArrayAdapter.CreateFromResource(MainActivity.context, Resource.Array.toArray, Resource.Layout.spinner);
			adapterTo.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

			fromSpinner.Adapter = adapterFrom;
			toSpinner.Adapter = adapterTo;

			currentFromStation = fromSpinner.SelectedItem.ToString();
			currentToStation = toSpinner.SelectedItem.ToString();

			fromSpinner.ItemSelected += async (sender, e) =>
			{
				if (fromSpinner.SelectedItem.ToString() != currentFromStation)
				{
					if (fromSpinner.SelectedItem.ToString() != currentToStation)
					{
						currentFromStation = fromSpinner.SelectedItem.ToString();
						await GetAdditionalTransportFeed(currentFromStation, currentToStation, DateTime.Now.ToString("yyyy-MM-dd"));
					}
					else 
					{
						Toast.MakeText(MainActivity.context, "Выберите другую станцию!", ToastLength.Short).Show();
					}
				}
			};

			toSpinner.ItemSelected += async (sender, e) =>
			{
				if (toSpinner.SelectedItem.ToString() != currentToStation)
				{
					if (toSpinner.SelectedItem.ToString() != currentFromStation)
					{
						currentToStation = toSpinner.SelectedItem.ToString();
						await GetAdditionalTransportFeed(currentFromStation, currentToStation, DateTime.Now.ToString("yyyy-MM-dd"));
					}
					else
					{
						Toast.MakeText(MainActivity.context, "Выберите другую станцию!", ToastLength.Short).Show();
					}
				}
			};

			recyclerView = root.FindViewById<RecyclerView>(Resource.Id.recyclerViewTransport);
			recyclerView.HasFixedSize = true;

			refresher = root.FindViewById<SwipeRefreshLayout>(Resource.Id.refresherTransport);
			refresher.Refresh += HandleRefresh;

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
		public async Task GetStartTransportFeed(string from, string to, string date)
		{
			try
			{
				var data = await JsonDataLoader.GetTransportData(from, to, date);
				transportFeed = data.ToList();
				transportAdapter = new TransportAdapter(transportFeed);
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

		/// <summary>
		/// Get news feed when app starts
		/// </summary>
		/// <returns>Array with start news feed</returns>
		public async Task GetAdditionalTransportFeed(string from, string to, string date)
		{
			try
			{
				refresher.Refreshing = true;
				var data = await JsonDataLoader.GetTransportData(from, to, date);
				transportFeed.Clear();
				transportFeed.AddRange(data);
				transportAdapter.NotifyDataSetChanged();
				refresher.Refreshing = false;
			}
			catch (System.ArgumentNullException)
			{
				refresher.Refreshing = false;
				//ShowNoInternetNofitication(root);
			}
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
				await GetAdditionalTransportFeed(currentFromStation, currentToStation, DateTime.Now.ToString("yyyy-MM-dd"));
			}
			else
			{
				Toast.MakeText(MainActivity.context, "Для загрузки новостей ребуется интернет соединение", ToastLength.Short).Show();
				refresher.Refreshing = false;
			}
		}

	}
}
