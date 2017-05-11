
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;


namespace Trilistnik
{
	public class SettingsFragment : Android.Support.V7.Preferences.PreferenceFragmentCompat
	{
		private static String ARG_TITLE = "Настройки";
		private String title;

		public static SettingsFragment NewInstance(String param1)
		{
			SettingsFragment fragment = new SettingsFragment();
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

		public override void OnCreatePreferences(Bundle savedInstanceState, string rootKey)
		{
			AddPreferencesFromResource(Resource.Xml.preferences);
		}
	}
}
