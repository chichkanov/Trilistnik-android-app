using System;
using System.Collections.Generic;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Load.Engine;

namespace Trilistnik
{
	public class TransportAdapter : RecyclerView.Adapter
	{
		private readonly List<TrainInfo> transportFeed;

		public TransportAdapter(List<TrainInfo> list)
		{
			this.transportFeed = list;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var transportViewHolder = (TransportViewHolder)holder;
			string arrivalText = transportFeed[position].Arrival.Split(' ')[1].Substring(0, 5);
			string departureText = transportFeed[position].Departure.Split(' ')[1].Substring(0, 5);
			string duration = "В пути: " + (int.Parse(transportFeed[position].Duration) / 60).ToString() + " минут";

			transportViewHolder.Arrival.Text = arrivalText;
			transportViewHolder.Departure.Text = departureText;
			transportViewHolder.Duration.Text = duration;
			transportViewHolder.Stops.Text = transportFeed[position].Stops;
			transportViewHolder.Title.Text = transportFeed[position].Title;
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.transport_item, parent, false);
			return new TransportViewHolder(layout);
		}

		public override int ItemCount
		{
			get { return transportFeed.Count; }
		}
	}
}
