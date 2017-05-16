using System;
using Android.App;
using Android.Widget;
using Firebase.Xamarin.Database;

namespace Trilistnik
{
	public class AddGoodDialogFragment : DialogFragment
	{
		public static String TAG = "AddGoodDialogFragment";
		public Action<GoodsItem> onItemAdded;

		public override void OnCreate(Android.OS.Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
			SetStyle(DialogFragmentStyle.Normal, Resource.Style.MyDialogFragmentStyle);
		}

		public override Android.Views.View OnCreateView(Android.Views.LayoutInflater inflater, Android.Views.ViewGroup container, Android.OS.Bundle savedInstanceState)
		{
			var v = inflater.Inflate(Resource.Layout.fragment_dialog_add_good, null);
			Dialog.SetTitle("Добавить вещь");
			EditText title = v.FindViewById<EditText>(Resource.Id.et_good_title);
			EditText desc = v.FindViewById<EditText>(Resource.Id.et_good_desc);
			ImageButton button = v.FindViewById<ImageButton>(Resource.Id.ib_add_good);
			button.Click += async (sender, e) =>
			{
				
				if (title.Text.Trim().Length > 0 && desc.Text.Trim().Length > 0)
				{
					DismissAllowingStateLoss();
					GoodsItem goodsItem = new GoodsItem(title.Text, desc.Text, DateTime.Now.ToString(),
												 MainActivity.prefs.GetString("userImg", null), MainActivity.prefs.GetString("userId", null));
					onItemAdded(goodsItem);
					try
					{
						var firebase = new FirebaseClient(ApiKeys.firebaseUrl);
						await firebase
							  .Child("Goods")
							.PostAsync(goodsItem);
					}
					catch (Exception exc)
					{
						Console.WriteLine(exc.Message);
					}
				}
				else
				{
					Toast.MakeText(Context, "Заполните все поля!", ToastLength.Short).Show();
				}
			};
			return v;
		}
	}
}
