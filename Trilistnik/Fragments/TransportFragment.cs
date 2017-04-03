
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
		private ImageButton switchTrainButton;
		private ViewGroup root;
		private Spinner fromSpinner, toSpinner;
		private string currentFromStation, currentToStation;
		private Button todayButton, tommorowButton;
		private LinearLayoutManager layoutManager;
		private ProgressBar loadingSpinner;

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
		}

		public override async void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);
			if (MainActivity.isOnline)
			{
				await GetStartTransportFeed(currentFromStation, currentToStation, DateTime.Now.ToString("yyyy-MM-dd"));
			}
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_transport, null);

			noInternetLayout = root.FindViewById<LinearLayout>(Resource.Id.noInternetContentTransport);
			todayButton = root.FindViewById<Button>(Resource.Id.buttonTrainToday);
			tommorowButton = root.FindViewById<Button>(Resource.Id.buttonTrainTommorow);
			fromSpinner = root.FindViewById<Spinner>(Resource.Id.fromSpinner);
			toSpinner = root.FindViewById<Spinner>(Resource.Id.toSpinner);
			switchTrainButton = root.FindViewById<ImageButton>(Resource.Id.buttonSwitchTrain);
			loadingSpinner = root.FindViewById<ProgressBar>(Resource.Id.loading_spinnerTransport);

			var adapterFrom = ArrayAdapter.CreateFromResource(MainActivity.context, Resource.Array.trainArray, Resource.Layout.spinner);
			adapterFrom.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

			var adapterTo = ArrayAdapter.CreateFromResource(MainActivity.context, Resource.Array.trainArray, Resource.Layout.spinner);
			adapterTo.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);

			fromSpinner.Adapter = adapterFrom;
			toSpinner.Adapter = adapterTo;
			fromSpinner.SetSelection(0);
			toSpinner.SetSelection(5);

			currentFromStation = fromSpinner.SelectedItem.ToString();
			currentToStation = toSpinner.SelectedItem.ToString();

			fromSpinner.ItemSelected += FromStationSelected;
			toSpinner.ItemSelected += ToStationSelected;
			switchTrainButton.Click += SwitchTrains;

			todayButton.Selected = true;
			todayButton.Click += TodayButtonClick;
			tommorowButton.Click += TommorowButtonClick;

			recyclerView = root.FindViewById<RecyclerView>(Resource.Id.recyclerViewTransport);
			recyclerView.HasFixedSize = true;

			refresher = root.FindViewById<SwipeRefreshLayout>(Resource.Id.refresherTransport);
			refresher.Refresh += HandleRefresh;

			layoutManager = new LinearLayoutManager(Activity);
			recyclerView.SetLayoutManager(layoutManager);
			transportAdapter = new TransportAdapter(transportFeed); 
			recyclerView.SetAdapter(transportAdapter);

			PerformAutoUpdate();

			if (!MainActivity.isOnline)
			{
				ShowNoInternetNofitication();
			}

			return root;
		}

		public override void OnResume()
		{
			base.OnResume();
			transportAdapter.NotifyDataSetChanged();
		}

		/// <summary>
		/// Get news feed when app starts
		/// </summary>
		/// <returns>Array with start news feed</returns>
		public async Task GetStartTransportFeed(string from, string to, string date)
		{
			try
			{
				loadingSpinner.Visibility = ViewStates.Visible;
				var data = await DataLoader.GetTransportData(from, to, date);
				transportFeed.AddRange(data);
				transportAdapter.NotifyDataSetChanged();
				if (!tommorowButton.Selected) recyclerView.ScrollToPosition(GetNearestTime());
				refresher.Refreshing = false;
				if (MainActivity.isOnline && noInternetLayout.Visibility == ViewStates.Visible)
				{
					noInternetLayout.Visibility = ViewStates.Gone;
				}
				refresher.Refreshing = false;
				loadingSpinner.Visibility = ViewStates.Gone;

			}
			catch (System.Net.Http.HttpRequestException)
			{
				refresher.Refreshing = false;
				loadingSpinner.Visibility = ViewStates.Gone;
				ShowNoInternetNofitication();
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
				loadingSpinner.Visibility = ViewStates.Visible;
				recyclerView.Visibility = ViewStates.Gone;
				var data = await DataLoader.GetTransportData(from, to, date);
				transportFeed.Clear();
				transportFeed.AddRange(data);
				transportAdapter.NotifyDataSetChanged();
				if (!tommorowButton.Selected)
				{
					layoutManager.ScrollToPositionWithOffset(GetNearestTime(), 0);
				}
				refresher.Refreshing = false;
				loadingSpinner.Visibility = ViewStates.Gone;
				recyclerView.Visibility = ViewStates.Visible;

			}
			catch (System.Net.Http.HttpRequestException)
			{
				refresher.Refreshing = false;
				loadingSpinner.Visibility = ViewStates.Gone;
				ShowNoInternetNofitication();
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
				refresher.Refreshing = true;
				if (todayButton.Selected) await GetAdditionalTransportFeed(currentFromStation, currentToStation, DateTime.Now.ToString("yyyy-MM-dd"));
				else await GetAdditionalTransportFeed(currentFromStation, currentToStation, DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
			}
			else
			{
				Toast.MakeText(MainActivity.context, "Для загрузки расписания ребуется интернет соединение", ToastLength.Short).Show();
				refresher.Refreshing = false;
			}
		}

		public async void FromStationSelected(Object sender, EventArgs e)
		{
			if (fromSpinner.SelectedItem.ToString() != currentFromStation)
			{
				if (fromSpinner.SelectedItem.ToString() != currentToStation)
				{
					currentFromStation = fromSpinner.SelectedItem.ToString();
					if(todayButton.Selected)await GetAdditionalTransportFeed(currentFromStation, currentToStation, DateTime.Now.ToString("yyyy-MM-dd"));
					else await GetAdditionalTransportFeed(currentFromStation, currentToStation, DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));

				}
				else
				{
					Toast.MakeText(MainActivity.context, "Выберите другую станцию!", ToastLength.Short).Show();
					currentFromStation = fromSpinner.SelectedItem.ToString();
				}
			}
		}

		public async void ToStationSelected(Object sender, EventArgs e)
		{
			if (toSpinner.SelectedItem.ToString() != currentToStation)
			{
				if (toSpinner.SelectedItem.ToString() != currentFromStation)
				{
					currentToStation = toSpinner.SelectedItem.ToString();
					if(todayButton.Selected) await GetAdditionalTransportFeed(currentFromStation, currentToStation, DateTime.Now.ToString("yyyy-MM-dd"));
					else await GetAdditionalTransportFeed(currentFromStation, currentToStation, DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));

				}
				else
				{
					Toast.MakeText(MainActivity.context, "Выберите другую станцию!", ToastLength.Short).Show();
					currentToStation = toSpinner.SelectedItem.ToString();
				}
			}
		}

		public async void TodayButtonClick(Object sender, EventArgs e)
		{
			if (tommorowButton.Selected)
			{
				todayButton.Selected = true;
				tommorowButton.Selected = false;
				await GetAdditionalTransportFeed(currentFromStation, currentToStation, DateTime.Now.ToString("yyyy-MM-dd"));
			}
		}

		public async void TommorowButtonClick(Object sender, EventArgs e)
		{
			if (todayButton.Selected)
			{
				todayButton.Selected = false;
				tommorowButton.Selected = true;
				await GetAdditionalTransportFeed(currentFromStation, currentToStation, DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
			}
		}

		public async void SwitchTrains(Object sender, EventArgs e)
		{
			if (currentToStation != currentFromStation)
			{
				int fromIndex = fromSpinner.SelectedItemPosition;
				fromSpinner.SetSelection(toSpinner.SelectedItemPosition);
				toSpinner.SetSelection(fromIndex);
				currentToStation = toSpinner.SelectedItem.ToString();
				currentFromStation = fromSpinner.SelectedItem.ToString();
				if (todayButton.Selected)
				{
					await GetAdditionalTransportFeed(currentFromStation, currentToStation, DateTime.Now.ToString("yyyy-MM-dd"));
				}
				else 
				{
					await GetAdditionalTransportFeed(currentFromStation, currentToStation, DateTime.Now.AddDays(1).ToString("yyyy-MM-dd"));
				}
			}
			else 
			{
				Toast.MakeText(MainActivity.context, "Выберите другую станцию!", ToastLength.Short).Show();
			}
		}

		private int GetNearestTime()
		{
			int position = 0;
			int curDate = int.Parse(DateTime.Now.ToString("HH:mm").Substring(0,2) + DateTime.Now.ToString("HH:mm").Substring(3));

			for (int i = 0; i < transportFeed.Count; i++)
			{
				string departureText = transportFeed[i].Departure.Split(' ')[1].Substring(0, 5);

				int trainDate = int.Parse(departureText.Substring(0, 2) + departureText.Substring(3));
				if (trainDate >= curDate)
				{
					position = i;
					break;
				}
			}

			return position;
		}

		//TODO FIX CONTENT HIDE WHEN OPEN WITH INTERNET SWITCH STATION AND PRESS UPDATE
		/// <summary>
		/// Show ui according to internet connection
		/// </summary>
		private void ShowNoInternetNofitication()
		{
			noInternetLayout.Visibility = ViewStates.Visible;
			for (int i = 0; i < noInternetLayout.ChildCount; i++)
			{
				noInternetLayout.GetChildAt(i).Visibility = ViewStates.Visible;
			}
			noInternetButton = root.FindViewById<Button>(Resource.Id.buttonRepeatConnectionTransport);
			noInternetButton.Click += async (sender, e) =>
			{
				if (MainActivity.isOnline)
				{
					noInternetLayout.Visibility = ViewStates.Gone;
					loadingSpinner.Visibility = ViewStates.Visible;
					await GetStartTransportFeed(currentFromStation, currentToStation, DateTime.Now.ToString("yyyy-MM-dd"));
					loadingSpinner.Visibility = ViewStates.Gone;
				}
			};
		}

		private void PerformAutoUpdate()
		{
			var timer = new System.Threading.Timer((e) =>
			{
				using (var h = new Handler(Looper.MainLooper))

					h.Post(() => { transportAdapter.NotifyDataSetChanged(); });

			}, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
		}

	}
}
