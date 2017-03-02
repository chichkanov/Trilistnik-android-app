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

namespace Trilistnik
{
	[Activity(Label = "Трилистник", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Icon = "@mipmap/icon", HardwareAccelerated = true)]
	public class MainActivity : AppCompatActivity
	{
		public static Context context;
		public static bool isOnline;

		private DrawerLayout drawerLayout;
		private SettingsFragment settingsFragment;
		private PayFragment payFragment;
		private FixFragment fixFragment;
		private NewsFragment newsFragment;
		private Fragment currentFragment;
		private Toolbar toolbar;
		private AppBarLayout appBarLayout;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			SetTheme(Resource.Style.mainAppTheme);
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main);

			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			appBarLayout = FindViewById<AppBarLayout>(Resource.Id.appBarLayout);
			drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);

			isOnline = checkConnection();
			InternetReceiver internetReceiver = new InternetReceiver();
			this.RegisterReceiver(internetReceiver, new IntentFilter(ConnectivityManager.ConnectivityAction));
			internetReceiver.InternetConnectionLost += (sender, e) => isOnline = false;
			internetReceiver.InternetConnectionReconnect += (sender, e) => isOnline = true;

			context = Application.Context;
			newsFragment = new NewsFragment();

			toolbar.Title = "Новости";
			SetSupportActionBar(toolbar);

			//Enable support action bar to display hamburger
			SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);

			navigationView.SetCheckedItem(Resource.Id.nav_news);
			SetupDrawerContent(navigationView);

			var trans = SupportFragmentManager.BeginTransaction();
			trans.Add(Resource.Id.content, newsFragment);
			currentFragment = newsFragment;
			trans.Commit();
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
						toolbar.Title = "Новости";
						ShowFragment(newsFragment);
						break;
					case Resource.Id.nav_pay:
						if (payFragment == null) payFragment = new PayFragment();
						toolbar.Title = "Оплата";
						ShowFragment(payFragment);
						break;
					case Resource.Id.nav_fix:
						if (fixFragment == null) fixFragment = new FixFragment();
						toolbar.Title = "Заявка на ремонт";
						ShowFragment(fixFragment);
						break;
					case Resource.Id.nav_settings:
						if (settingsFragment == null) settingsFragment = new SettingsFragment();
						toolbar.Title = "Настройки";
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
	}
}

