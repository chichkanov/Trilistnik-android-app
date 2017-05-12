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
		private Toolbar toolbar;
		private Account account;

		public static ISharedPreferences prefs;
		public static ISharedPreferencesEditor prefsEditor;

		public const int NEWS_FRAGMENT = 1;
		public const int TRANSPORT_FRAGMENT = 2;

		protected async override void OnCreate(Bundle savedInstanceState)
		{
			SetTheme(Resource.Style.mainAppTheme);
			base.OnCreate(savedInstanceState);
			SetContentView(Resource.Layout.activity_main);

			toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
			SetSupportActionBar(toolbar);
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
				isOnline = CheckConnection();
				ChooseDefaultFragment();
				SetupDrawerContent(navigationView);
			}

			if (prefs.GetBoolean("isLoggedIn", false))
			{
				var header = navigationView.GetHeaderView(0);
				var userName = (TextView)header.FindViewById(Resource.Id.tv_header_text);
				userName.Text = await GetUserName();
				SetUserPhoto(header);
			}

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
		/// <param name="navView">Navigation view</param>
		private void SetupDrawerContent(NavigationView navView)
		{

			navView.NavigationItemSelected += (sender, e) =>
			{
				e.MenuItem.SetChecked(true);
				switch (e.MenuItem.ItemId)
				{
					case Resource.Id.nav_news:
						NewsFragment newsFramgent = NewsFragment.NewInstance("Новости");
						ShowFragment(newsFramgent);
						break;
					case Resource.Id.nav_transport:
						TransportFragment transportFragment = TransportFragment.NewInstance("Транспорт");
						ShowFragment(transportFragment);
						break;
					case Resource.Id.nav_pay:
						PayFragment payFragment = PayFragment.NewInstance("Оплата");
						ShowFragment(payFragment);
						break;
					case Resource.Id.nav_fix:
						FixFragment fixFragment = FixFragment.NewInstance("Заявка на ремонт");
						ShowFragment(fixFragment);
						break;
					case Resource.Id.nav_settings:
						SettingsFragment settingsFragment = SettingsFragment.NewInstance("Настройки");
						ShowFragment(settingsFragment);
						break;
					case Resource.Id.nav_useful:
						UsefulFragment usefulFragment = UsefulFragment.NewInstance("Полезное");
						ShowFragment(usefulFragment);
						break;
					case Resource.Id.nav_goods:
						GoodsFragment goodsFragment = GoodsFragment.NewInstance("Барахолка");
						ShowFragment(goodsFragment);
						break;
					case Resource.Id.nav_serv:
						ServicesFragment serviceFragment = ServicesFragment.NewInstance("Услуги");
						ShowFragment(serviceFragment);
						break;
					case Resource.Id.nav_my_posts:
						MyPostsFragment myPostsFragment = MyPostsFragment.NewInstance("Мои публикации");
						ShowFragment(myPostsFragment);
						break;
					case Resource.Id.nav_meetups:
						MeetupsFragment meetupsFragment = MeetupsFragment.NewInstance("Мероприятия");
						ShowFragment(meetupsFragment);
						break;	
				}

				Handler h = new Handler();
				h.PostDelayed(new Action(drawerLayout.CloseDrawers), 30);
			};
		}

		/// <summary>
		/// Show chosen fragment
		/// </summary>
		/// <param name="fragment">Fragment</param>
		private void ShowFragment(Fragment fragment)
		{
			var trans = SupportFragmentManager.BeginTransaction();
			Console.WriteLine();
			trans.SetCustomAnimations(Resource.Animation.enter, Resource.Animation.exit);
			trans.Replace(Resource.Id.content, fragment);
			trans.Commit();
		}

		/// <summary>
		/// CCheck internet connection
		/// </summary>
		/// <returns><c>true</c>If online<c>false</c>If offline</returns>
		public bool CheckConnection()
		{
			ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
			NetworkInfo networkInfo = connectivityManager.ActiveNetworkInfo;
			if (networkInfo != null && networkInfo.IsConnectedOrConnecting) return true;
			else return false;
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
					NewsFragment newsFramgent = NewsFragment.NewInstance("Новости");
					ShowFragment(newsFramgent);
					navigationView.SetCheckedItem(Resource.Id.nav_news);
					break;
				case TRANSPORT_FRAGMENT:
					TransportFragment transportFragment = TransportFragment.NewInstance("Транспорт");
					ShowFragment(transportFragment);
					navigationView.SetCheckedItem(Resource.Id.nav_transport);
					break;
			}
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

