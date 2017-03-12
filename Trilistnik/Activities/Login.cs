using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Trilistnik
{
	[Activity(Label = "Трилистник", MainLauncher = true)]
	public class LoginActivity : Activity
	{
		protected override void OnCreate(Bundle bundle)
		{
			SetTheme(Resource.Style.mainAppTheme);
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Login);

			Button login = FindViewById<Button>(Resource.Id.login);
			Button skipLogin = FindViewById<Button>(Resource.Id.skipLogin);
			Intent intent = new Intent(this, typeof(MainActivity));

			login.Click += (object sender, EventArgs e) =>
			{
				Toast.MakeText(this, "Login Button Clicked!", ToastLength.Short).Show();
			};

			skipLogin.Click += (object sender, EventArgs e) =>
			{
				Finish();
				StartActivity(intent);
			};
		}
	}
}