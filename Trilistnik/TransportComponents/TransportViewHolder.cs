﻿using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Trilistnik
{
	public class TransportViewHolder : RecyclerView.ViewHolder
	{
		public TextView Arrival { get; set; }
		public TextView Departure { get; set; }
		public TextView Duration { get; set; }
		public TextView Title { get; set; }
		public TextView Stops { get; set; }


		public TransportViewHolder(View itemView) : base(itemView)
		{
			Arrival = itemView.FindViewById<TextView>(Resource.Id.transportArrival);
			Departure = itemView.FindViewById<TextView>(Resource.Id.transportDeparture);
			Duration = itemView.FindViewById<TextView>(Resource.Id.transportDuration);
			Title = itemView.FindViewById<TextView>(Resource.Id.transportTitle);
			Stops = itemView.FindViewById<TextView>(Resource.Id.transportStops);
		}
	}
}
