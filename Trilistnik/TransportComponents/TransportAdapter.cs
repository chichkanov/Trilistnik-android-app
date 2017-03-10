using System;
using System.Collections.Generic;
using Android.Content;
using Android.Graphics;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
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
			string duration = (int.Parse(transportFeed[position].Duration) / 60).ToString() + " мин";

			transportViewHolder.Arrival.Text = arrivalText;
			transportViewHolder.Departure.Text = departureText;
			transportViewHolder.Duration.Text = duration;
			transportViewHolder.Stops.Text = transportFeed[position].Stops;
			transportViewHolder.Title.Text = transportFeed[position].Title;
			if (transportFeed[position].Express != String.Empty)
			{
				transportViewHolder.IsExpress.Visibility = ViewStates.Visible;
				transportViewHolder.IsExpress.Text = "Экспресс";
			}
			else {
				transportViewHolder.IsExpress.Visibility = ViewStates.Gone;

			}
			if (transportFeed[position].StandartPlus != String.Empty)
			{
				transportViewHolder.IsStandartPlus.Visibility = ViewStates.Visible;
				transportViewHolder.IsStandartPlus.Text = "Стандарт плюс";
			}
			else {
				transportViewHolder.IsStandartPlus.Visibility = ViewStates.Gone;

			}

			int curDate = int.Parse(DateTime.Now.ToString("HH:mm").Substring(0, 2) + DateTime.Now.ToString("HH:mm").Substring(3));
			int trainDate = int.Parse(departureText.Substring(0, 2) + departureText.Substring(3));

			int curDay = DateTime.Now.Day;
			int trainDay = int.Parse(transportFeed[position].Departure.Substring(8, 2));

			if (curDay == trainDay)
			{
				if (curDate < trainDate)
				{
					if (position % 2 == 0) transportViewHolder.ll.SetBackgroundColor(Color.Rgb(250, 250, 250));
					else {
						transportViewHolder.ll.SetBackgroundColor(Color.Rgb(238, 238, 238));
					}
				}
				else {
					transportViewHolder.ll.SetBackgroundColor(Color.Rgb(210, 210, 210));
				}
			}
			else {
				if (position % 2 == 0) transportViewHolder.ll.SetBackgroundColor(Color.Rgb(250, 250, 250));
				else {
					transportViewHolder.ll.SetBackgroundColor(Color.Rgb(238, 238, 238));
				}
			}


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
