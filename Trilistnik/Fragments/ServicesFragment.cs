
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
using Android.Support.Design.Widget;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase.Xamarin.Database;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Trilistnik
{
	public class ServicesFragment : Android.Support.V4.App.Fragment
	{
		private static String ARG_TITLE = "Услуги";
		private String title;

		private RecyclerView recyclerView;
		private GoodsAdapter adapter;
		private List<GoodsItem> dataset = new List<GoodsItem>();
		private ViewGroup root;
		private ProgressBar loadingSpinner;

		public static ServicesFragment NewInstance(String param1)
		{
			ServicesFragment fragment = new ServicesFragment();
			Bundle args = new Bundle();
			args.PutString(ARG_TITLE, param1);
			fragment.Arguments = args;
			return fragment;
		}

		public override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			if (Arguments != null)
			{
				title = Arguments.GetString(ARG_TITLE);
			}
		}

		public override async void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);
			Activity.Title = title;
			await GetDataset();
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_goods, null);
			var fab = root.FindViewById<FloatingActionButton>(Resource.Id.fab_add_good);
			if (!MainActivity.prefs.GetBoolean("isLoggedIn", false)) fab.SetVisibility(ViewStates.Gone);
			loadingSpinner = root.FindViewById<ProgressBar>(Resource.Id.loading_goods);
			fab.Click += (sender, e) =>
			{
				AddServiceDialogFragment dialogFragment = new AddServiceDialogFragment();
				dialogFragment.onItemAdded += (GoodsItem obj) =>
				{
					dataset.Add(obj);
					adapter.NotifyDataSetChanged();
				};
				dialogFragment.Show(Activity.FragmentManager, AddServiceDialogFragment.TAG);
			};
			recyclerView = root.FindViewById<RecyclerView>(Resource.Id.rv_goods);
			InitRecyclerView();

			return root;
		}

		private void InitRecyclerView()
		{
			adapter = new GoodsAdapter(dataset);
			var layoutManager = new LinearLayoutManager(Activity);
			recyclerView.SetLayoutManager(layoutManager);
			recyclerView.SetAdapter(adapter);
			DividerItemDecoration decor = new DividerItemDecoration(Activity, layoutManager.Orientation);
			recyclerView.AddItemDecoration(decor);
		}

		private async Task GetDataset()
		{
			try
			{
				loadingSpinner.Visibility = ViewStates.Visible;
				var firebase = new FirebaseClient(ApiKeys.firebaseUrl);
				var items = await firebase
				  .Child("Services")
				  .OnceAsync<GoodsItem>();

				foreach (var item in items)
				{
					dataset.Add(item.Object);
				}
				adapter.NotifyDataSetChanged();
			}
			catch (Exception e)
			{
				Console.Write(e.Message);
			}
			finally
			{
				loadingSpinner.Visibility = ViewStates.Gone;
			}
		}

	}
}
