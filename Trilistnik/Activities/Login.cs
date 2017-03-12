using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Xamarin.Auth;

namespace Trilistnik
{
	[Activity(Label = "Трилистник", MainLauncher = true)]
	public class LoginActivity : Activity
	{
		public string token;
		public string userId;

		protected override void OnCreate(Bundle bundle)
		{
			SetTheme(Resource.Style.mainAppTheme);
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Login);

			Button login = FindViewById<Button>(Resource.Id.login);
			Button skipLogin = FindViewById<Button>(Resource.Id.skipLogin);
			Intent intent = new Intent(this, typeof(MainActivity));

			login.Click += delegate { LoginToVk(); };

			skipLogin.Click += (object sender, EventArgs e) =>
			{
				Finish();
				StartActivity(intent);
			};
		}

		private void LoginToVk()
		{
			var auth = new OAuth2Authenticator(
				clientId: "5920615",
				scope: "friends,video,groups",
				authorizeUrl: new Uri("https://oauth.vk.com/authorize"),
				redirectUrl: new Uri("https://oauth.vk.com/blank.html"));
			auth.AllowCancel = true;
			auth.Completed += (sender, ee) =>
			{
				if (!ee.IsAuthenticated)
				{
					var builder = new AlertDialog.Builder(this);
					builder.SetMessage("Not Authenticated");
					builder.SetPositiveButton("Ok", (o, e) => { });
					builder.Create().Show();
					return;
				}
				else
				{
					token = ee.Account.Properties["access_token"].ToString();
					userId = ee.Account.Properties["user_id"].ToString();
				}
			};
			var intent = auth.GetUI(this);
			//StartActivity(intent);
		}
	}
}