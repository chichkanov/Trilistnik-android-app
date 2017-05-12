
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
	public class MyPostsFragment : Android.Support.V4.App.Fragment
	{
		private static String ARG_TITLE = "Мои публикации";
		private String title;

		private RecyclerView recyclerView;
		private MyPostsAdapter adapter;
		private List<GoodsItem> dataset = new List<GoodsItem>();

		private ViewGroup root;
		private ProgressBar loadingSpinner;

		public static MyPostsFragment NewInstance(String param1)
		{
			MyPostsFragment fragment = new MyPostsFragment();
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
			root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_my_posts, null);
			loadingSpinner = root.FindViewById<ProgressBar>(Resource.Id.loading_posts);
			recyclerView = root.FindViewById<RecyclerView>(Resource.Id.rv_my_posts);

			InitRecyclerView();

			return root;
		}

		private void InitRecyclerView()
		{
			adapter = new MyPostsAdapter(dataset);
			adapter.ItemClick += async (sender, e) =>
			{
				await DeleteItem(dataset[e]);
			};
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
				  .Child("Goods")
				  .OnceAsync<GoodsItem>();
				

				foreach (var item in items)
				{
					if(item.Object.Id.Equals(MainActivity.prefs.GetString("userId", "")))dataset.Add(item.Object);
				}

				var items2 = await firebase
				  .Child("Services")
				  .OnceAsync<GoodsItem>();

				foreach (var item in items2)
				{
					if (item.Object.Id.Equals(MainActivity.prefs.GetString("userId", "")))dataset.Add(item.Object);
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

		private async Task DeleteItem(GoodsItem goodsItem)
		{
			dataset.Remove(goodsItem);
			adapter.NotifyDataSetChanged();

			var firebase = new FirebaseClient(ApiKeys.firebaseUrl);

			var items1 = await firebase
				  .Child("Goods")
				  .OnceAsync<GoodsItem>();
			
			var items2 = await firebase
				  .Child("Services")
				  .OnceAsync<GoodsItem>();

			String itemToDelete = "";

			foreach (var item in items1)
			{
				var obj = item.Object;
				if (goodsItem.Title.Equals(obj.Title) && goodsItem.Desc.Equals(obj.Desc)
				   && goodsItem.Date.Equals(obj.Date) && goodsItem.Id.Equals(obj.Id))
					itemToDelete = "Goods/" + item.Key;
			}

			foreach (var item in items2)
			{
				var obj = item.Object;
				if (goodsItem.Title.Equals(obj.Title) && goodsItem.Desc.Equals(obj.Desc)
				   && goodsItem.Date.Equals(obj.Date) && goodsItem.Id.Equals(obj.Id))
					itemToDelete = "Services/" + item.Key;
			}

			await firebase
				.Child(itemToDelete)
				.DeleteAsync();
		}
	}
}
