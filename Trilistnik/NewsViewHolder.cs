using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Trilistnik
{
	public class NewsViewHolder : RecyclerView.ViewHolder
	{
		public TextView NewsText { get; set; }
		public TextView NewsDate { get; set; }
		public ImageView NewsImg { get; set; }


		public NewsViewHolder(View itemView, Action<int> listener) : base(itemView)
        {
			NewsText = itemView.FindViewById<TextView>(Resource.Id.newsText);
			NewsDate = itemView.FindViewById<TextView>(Resource.Id.newsDate);
			NewsImg = itemView.FindViewById<ImageView>(Resource.Id.newsImg);

			itemView.Click += (sender, e) => listener(Position);
		}
	}
}
