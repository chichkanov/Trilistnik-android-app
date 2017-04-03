using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V4.Widget;
using Android.Support.Design.Widget;
using Toolbar = Android.Support.V7.Widget.Toolbar;
using Android.Views;
using Fragment = Android.Support.V4.App.Fragment;
using Android.Net;
using System;
using Android.Content;
using Android.Widget;
using Java.Lang;
using System.Collections.Generic;
using Xamarin.Auth;
using System.Linq;
using Citrina;
using System.Threading.Tasks;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Load.Engine;
using Com.Bumptech.Glide.Request.Target;
using Android.Graphics;
using Android.Support.V4.Graphics.Drawable;
using Android.Preferences;

namespace Trilistnik
{
	[Activity(Label = "Трилистник", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Icon = "@mipmap/icon", HardwareAccelerated = true)]
	public class MainActivity : AppCompatActivity
	{
		public static Context context;
		public static bool isOnline;

		private DrawerLayout drawerLayout;
		private NavigationView navigationView;
		private SettingsFragment settingsFragment;
		private PayFragment payFragment;
		private FixFragment fixFragment;
		private NewsFragment newsFragment;
		private TransportFragment transportFragment;
		private Fragment currentFragment;
		private Toolbar toolbar;
		private AppBarLayout appBarLayout;
		private Account account;

		private static ISharedPreferences prefs;
		private static ISharedPreferencesEditor prefsEditor;

		public const int NEWS_FRAGMENT = 1;
		public const int TRANSPORT_FRAGMENT = 2;

		protected async override void OnCreate(Bundle savedInstanceState)
		{
			SetTheme(Resource.Style.mainAppTheme);
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_main);


			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetSupportActionBar(toolbar);
			appBarLayout = FindViewById<AppBarLayout>(Resource.Id.appBarLayout);
			drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

			InternetReceiver internetReceiver = new InternetReceiver();
			this.RegisterReceiver(internetReceiver, new IntentFilter(ConnectivityManager.ConnectivityAction));
			internetReceiver.InternetConnectionLost += (sender, e) => isOnline = false;
			internetReceiver.InternetConnectionReconnect += (sender, e) => isOnline = true;

			context = Application.Context;
			InitPrefs();

			SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);

			if (savedInstanceState == null)
			{
				isOnline = checkConnection();
				ChooseDefaultFragment();
				SetupDrawerContent(navigationView);
			}
			var header = navigationView.GetHeaderView(0);
			var userName = (TextView)header.FindViewById(Resource.Id.tv_header_text);
			userName.Text = await GetUserName();
			SetUserPhoto(header);

		}

		/// <summary>
		/// If drawer open button pressed
		/// </summary>
		/// <returns><c>true</c>Selected<c>false</c>Otherwise</returns>
		/// <param name="item">Item</param>
		public override bool OnOptionsItemSelected(IMenuItem item)
		{
			switch (item.ItemId)
			{
				case Android.Resource.Id.Home:
					drawerLayout.OpenDrawer(Android.Support.V4.View.GravityCompat.Start);
					return true;
				default:
					return base.OnOptionsItemSelected(item);
			}
		}


		/// <summary>
		/// Set drawer content and add item selected listener
		/// </summary>
		/// <param name="navigationView">Navigation view</param>
		private void SetupDrawerContent(NavigationView navigationView)
		{

			navigationView.NavigationItemSelected += (sender, e) =>
			{
				e.MenuItem.SetChecked(true);
				switch (e.MenuItem.ItemId)
				{
					case Resource.Id.nav_news:
						if (newsFragment == null) newsFragment = new NewsFragment();
						Title = "Новости";
						ShowFragment(newsFragment);
						break;
					case Resource.Id.nav_transport:
						if (transportFragment == null) transportFragment = new TransportFragment();
						Title = "Транспорт";
						ShowFragment(transportFragment);
						break;
					case Resource.Id.nav_pay:
						if (payFragment == null) payFragment = new PayFragment();
						Title = "Оплата";
						ShowFragment(payFragment);
						break;
					case Resource.Id.nav_fix:
						if (fixFragment == null) fixFragment = new FixFragment();
						Title = "Заявка на ремонт";
						ShowFragment(fixFragment);
						break;
					case Resource.Id.nav_settings:
						if (settingsFragment == null) settingsFragment = new SettingsFragment();
						Title = "Настройки";
						ShowFragment(settingsFragment);
						break;
				}
				drawerLayout.CloseDrawers();
			};
		}

		/// <summary>
		/// Show chosen fragment
		/// </summary>
		/// <param name="fragment">Fragment</param>
		private void ShowFragment(Fragment fragment)
		{
			if (fragment.IsVisible) return;
			var trans = SupportFragmentManager.BeginTransaction();
			trans.SetCustomAnimations(Resource.Animation.enter, Resource.Animation.exit);
			trans.Hide(currentFragment);
			if (fragment.IsAdded) trans.Show(fragment);
			else trans.Add(Resource.Id.content, fragment);
			currentFragment = fragment;
			appBarLayout.SetExpanded(true);
			trans.Commit();
		}

		/// <summary>
		/// CCheck internet connection
		/// </summary>
		/// <returns><c>true</c>If online<c>false</c>If offline</returns>
		/// <param name="context">Contex.</param>
		public static bool checkConnection()
		{
			Runtime runtime = Runtime.GetRuntime();
			Java.Lang.Process ipProcess = runtime.Exec("/system/bin/ping -c 1 8.8.8.8");
			int exitValue = ipProcess.WaitFor();
			return exitValue == 0;
		}

		/// <summary>
		/// Chooses the default fragment
		/// </summary>
		private void ChooseDefaultFragment()
		{
			int res = int.Parse(prefs.GetString("defaultFragment", "1"));

			switch (res)
			{
				case NEWS_FRAGMENT:
					newsFragment = new NewsFragment();
					navigationView.SetCheckedItem(Resource.Id.nav_news);
					currentFragment = newsFragment;
					Title = "Новости";
					break;
				case TRANSPORT_FRAGMENT:
					transportFragment = new TransportFragment();
					navigationView.SetCheckedItem(Resource.Id.nav_transport);
					currentFragment = transportFragment;
					Title = "Транспорт";
					break;
			}

			var trans = SupportFragmentManager.BeginTransaction();
			trans.Add(Resource.Id.content, currentFragment);
			trans.Commit();
		}

		/// <summary>
		/// Gets the name of the user
		/// </summary>
		/// <returns>The user name</returns>
		private async Task<string> GetUserName()
		{
			if (prefs.GetBoolean("isUserNameCached", false))
			{
				return prefs.GetString("userName", "Аноним Анонимный");
			}
			account = AccountStore.Create().FindAccountsForService("vk").FirstOrDefault();
			var client = new CitrinaClient();
			var token = new UserAccessToken(account.Properties["access_token"], 999999, int.Parse(account.Properties["user_id"]), 5920615);
			var call = await client.Users.Get(new Citrina.StandardApi.Models.UsersGetRequest { AccessToken = token }).ConfigureAwait(false);
			var result = call.Response.First();
			prefsEditor.PutString("userName", result.FirstName + " " + result.LastName);
			prefsEditor.PutBoolean("isUserNameCached", true);
			prefsEditor.Apply();
			return result.FirstName + " " + result.LastName;
		}

		/// <summary>
		/// Gets the user photo
		/// </summary>
		/// <returns>The user photo</returns>
		private async Task<string> GetUserPhoto()
		{
			if (prefs.GetBoolean("isUserImgCached", false))
			{
				return prefs.GetString("userImg", "Надеюсь сюда не дойдет");
			}
			account = AccountStore.Create().FindAccountsForService("vk").FirstOrDefault();
			var client = new CitrinaClient();
			var token = new UserAccessToken(account.Properties["access_token"], 999999, int.Parse(account.Properties["user_id"]), 5920615);
			List<string> list = new List<string>();
			list.Add("photo_200");
			IEnumerable<string> fields = list;
			var call = await client.Users.Get(new Citrina.StandardApi.Models.UsersGetRequest { AccessToken = token, Fields = fields }).ConfigureAwait(false);
			var result = call.Response.First();
			prefsEditor.PutString("userImg", result.Photo200);
			prefsEditor.PutBoolean("isUserImgCached", true);
			prefsEditor.Apply();
			return result.Photo200;
		}

		//TODO When user change avatar but in-app avatar do not change because of the UserImg flag
		/// <summary>
		/// Sets the user photo
		/// </summary>
		/// <param name="header">Header view</param>
		private async void SetUserPhoto(View header)
		{
			var userImg = (ImageView)header.FindViewById(Resource.Id.tv_header_userPhoro);

			var photoUrl = await GetUserPhoto();
			Glide.With(this).Load(photoUrl)
			     .DiskCacheStrategy(DiskCacheStrategy.All)
				 .Override(200, 200)
				 .Into(userImg);
		}

		/// <summary>
		/// Inits the prefs
		/// </summary>
		private void InitPrefs()
		{
			prefs = PreferenceManager.GetDefaultSharedPreferences(this);
			prefsEditor = prefs.Edit();
			prefsEditor.PutBoolean("firstStart", false);
			prefsEditor.Apply();
		}
	}
}

