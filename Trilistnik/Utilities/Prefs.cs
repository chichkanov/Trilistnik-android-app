using System;
using Android.Content;
using Android.Preferences;

namespace Trilistnik
{
	public class Prefs
	{
		public static bool isFirstStart;

		private Context context;
		private static ISharedPreferences prefs;
		private static ISharedPreferencesEditor prefsEditor;

		public Prefs(Context context)
		{
			this.context = context;
			prefs = PreferenceManager.GetDefaultSharedPreferences(context);
			prefsEditor = prefs.Edit();
			isFirstStart = prefs.GetBoolean("firstStart", true);
			prefsEditor.PutBoolean("firstStart", false);
			prefsEditor.Apply();
		}
	}
}
