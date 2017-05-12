using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Refractored.Controls;

namespace Trilistnik
{
	public class GoodsViewHolder : RecyclerView.ViewHolder
	{
		public TextView Title { get; set; }
		public TextView Desc { get; set; }
		public TextView Date { get; set; }
		public ImageView Img { get; set; }
		public ImageButton SendMsg { get; set; }

		public GoodsViewHolder(View itemView, Action<int> sendListener) : base(itemView)
        {
			Title = itemView.FindViewById<TextView>(Resource.Id.tv_goods_title);
			Desc = itemView.FindViewById<TextView>(Resource.Id.tv_goods_desc);
			Date = itemView.FindViewById<TextView>(Resource.Id.tv_goods_date);
			Img = itemView.FindViewById<ImageView>(Resource.Id.iv_goods_photo);
			SendMsg = itemView.FindViewById<ImageButton>(Resource.Id.ib_goods_write);
			SendMsg.Click += (sender, e) => sendListener(base.Position);
		}
	}
}