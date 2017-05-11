using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Auth;
using Android.Preferences;

namespace Trilistnik
{
	[Activity(Label = "Трилистник", MainLauncher = true)]
	public class LoginActivity : Activity
	{
		public string token;
		public string userId;

		protected override void OnCreate(Bundle bundle)
		{

			if (PreferenceManager.GetDefaultSharedPreferences(this).GetBoolean("isLoggedIn", false))
			{
				StartNextScreen();
			}

			base.OnCreate(bundle);
			SetTheme(Resource.Style.mainAppTheme);
			SetContentView(Resource.Layout.activity_login);

			Button login = FindViewById<Button>(Resource.Id.login);
			Button skipLogin = FindViewById<Button>(Resource.Id.skipLogin);

			login.Click += delegate { LoginToVk(); };

			skipLogin.Click += (object sender, EventArgs e) =>
			{
				StartNextScreen();
			};
		}

		private void LoginToVk()
		{
			var auth = new OAuth2Authenticator(
				clientId: "5920615",
				scope: "",
				authorizeUrl: new Uri("https://oauth.vk.com/authorize"),
				redirectUrl: new Uri("https://oauth.vk.com/blank.html"));
			auth.AllowCancel = true;
			auth.IsUsingNativeUI = false;
			auth.Completed += (sender, ee) =>
			{
				if (!ee.IsAuthenticated)
				{
					var builder = new AlertDialog.Builder(this);
					builder.SetMessage("Ошибка!");
					builder.SetPositiveButton("Ок", (o, e) => { });
					builder.Create().Show();
					return;
				}
				else
				{
					token = ee.Account.Properties["access_token"].ToString();
					userId = ee.Account.Properties["user_id"].ToString();
					AccountStore.Create(this).Save(ee.Account, "vk");
					ISharedPreferencesEditor edit = PreferenceManager.GetDefaultSharedPreferences(this).Edit();
					edit.PutBoolean("isLoggedIn", true);
					edit.PutString("userId", ee.Account.Properties["user_id"]);
					edit.Apply();
					StartNextScreen();
				}
			};
			System.Object ui_intent_as_object = auth.GetUI(this);
			if (auth.IsUsingNativeUI == true)
			{
				global::Android.Support.CustomTabs.CustomTabsIntent cti = null;
				cti = (global::Android.Support.CustomTabs.CustomTabsIntent)ui_intent_as_object;
			}
			else
			{
				global::Android.Content.Intent i = null;
				i = (global::Android.Content.Intent)ui_intent_as_object;
				StartActivity(i);
			}
			var intent = auth.GetUI(this);
		}

		private void StartNextScreen()
		{
			Intent intent = new Intent(this, typeof(MainActivity));
			StartActivity(intent);
			Finish();

		}
	}
}