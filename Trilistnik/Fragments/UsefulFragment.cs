using System;
using Android.OS;
using Android.Views;

namespace Trilistnik
{
	public class UsefulFragment : Android.Support.V4.App.Fragment
	{
		private static String ARG_TITLE = "Полезное";
		private String title;

		public static UsefulFragment NewInstance(String param1)
		{
			UsefulFragment fragment = new UsefulFragment();
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

		public override void OnActivityCreated(Bundle savedInstanceState)
		{
			base.OnActivityCreated(savedInstanceState);
			Activity.Title = title;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var root = (ViewGroup)inflater.Inflate(Resource.Layout.fragment_useful, null);

			return root;
		}
	}
}
