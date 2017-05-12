using System;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Refractored.Controls;

namespace Trilistnik
{
	public class MyPostsViewHolder : RecyclerView.ViewHolder
	{
		public TextView Title { get; set; }
		public TextView Desc { get; set; }
		public TextView Date { get; set; }
		public ImageView Img { get; set; }
		public ImageButton Delete { get; set; }

		public MyPostsViewHolder(View itemView, Action<int> deleteListener) : base(itemView)
		{
			Title = itemView.FindViewById<TextView>(Resource.Id.tv_my_posts_title);
			Desc = itemView.FindViewById<TextView>(Resource.Id.tv_my_posts_desc);
			Date = itemView.FindViewById<TextView>(Resource.Id.tv_my_posts_date);
			Img = itemView.FindViewById<ImageView>(Resource.Id.iv_my_posts_photo);
			Delete = itemView.FindViewById<ImageButton>(Resource.Id.ib_my_posts_delete);

			Delete.Click += (sender, e) => deleteListener(base.Position);
		}
	}
}