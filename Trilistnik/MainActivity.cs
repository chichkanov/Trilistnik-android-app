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

namespace Trilistnik
{
	[Activity(Label = "Трилистник", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait, Icon = "@mipmap/icon", HardwareAccelerated = true)]
	public class MainActivity : AppCompatActivity
	{
		public static Context context;
		private DrawerLayout drawerLayout;
		private SettingsFragment settingsFragment;
		private PayFragment payFragment;
		private FixFragment fixFragment;
		private NewsFragment newsFragment;
		private Fragment currentFragment;
		private Toolbar toolbar;
		private AppBarLayout appBarLayout;

		protected async override void OnCreate(Bundle savedInstanceState)
		{
			SetTheme(Resource.Style.mainAppTheme);
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.Main);
			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			appBarLayout = FindViewById<AppBarLayout>(Resource.Id.appBarLayout);
			toolbar.Title = "Новости";
			SetSupportActionBar(toolbar);

			context = Application.Context;
			settingsFragment = new SettingsFragment();
			payFragment = new PayFragment();
			newsFragment = new NewsFragment();
			fixFragment = new FixFragment();

			//Enable support action bar to display hamburger
			SupportActionBar.SetHomeAsUpIndicator(Resource.Drawable.ic_menu);
			SupportActionBar.SetDisplayHomeAsUpEnabled(true);

			drawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);

			var navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
			navigationView.SetCheckedItem(Resource.Id.nav_news);

			SetupDrawerContent(navigationView);

			var trans = SupportFragmentManager.BeginTransaction();
			trans.Add(Resource.Id.content, settingsFragment);
			trans.Hide(settingsFragment);
			trans.Add(Resource.Id.content, payFragment);
			trans.Hide(payFragment);
			trans.Add(Resource.Id.content, fixFragment);
			trans.Hide(fixFragment);
			trans.Add(Resource.Id.content, newsFragment);
			currentFragment = newsFragment;
			trans.Commit();
		}

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
						toolbar.Title = "Оплата";
						ShowFragment(payFragment);
						break;
					case Resource.Id.nav_fix:
						toolbar.Title = "Заявка на ремонт";
						ShowFragment(fixFragment);
						break;	
					case Resource.Id.nav_settings:
						toolbar.Title = "Настройки";
						ShowFragment(settingsFragment);
						break;	
				}
				drawerLayout.CloseDrawers();
			};
		}

		private void ShowFragment(Fragment fragment)
		{
			var trans = SupportFragmentManager.BeginTransaction();
			trans.SetCustomAnimations(Resource.Animation.enter, Resource.Animation.exit);
			trans.Hide(currentFragment);
			appBarLayout.SetExpanded(true);
			trans.Show(fragment);
			trans.Commit();
			currentFragment = fragment;
		}

		public static bool isOnline(Context context)
		{
			ConnectivityManager connectivityManager = (ConnectivityManager)context.GetSystemService(ConnectivityService);
			NetworkInfo networkInfo = connectivityManager.ActiveNetworkInfo;
			return networkInfo != null && networkInfo.IsConnected;
		}
	}
}

