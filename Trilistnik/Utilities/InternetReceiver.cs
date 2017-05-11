using System;
using Android.App;
using Android.Content;
using Android.Net;
using Android.Widget;

namespace Trilistnik
{
	public class InternetReceiver : BroadcastReceiver
	{
		public event EventHandler InternetConnectionLost;
		public event EventHandler InternetConnectionReconnect;

		public override void OnReceive(Context context, Intent intent)
		{
			if (!MainActivity.isOnline)
			{
				if (InternetConnectionLost != null)
				{
					InternetConnectionLost(this, null);
				}
			}
			else {
				if (InternetConnectionReconnect != null)
				{
					InternetConnectionReconnect(this, null);
				}
			}

		}
	}
}