using System;
using System.Collections.Generic;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Load.Engine;

namespace Trilistnik
{
	public class NewsAdapter : RecyclerView.Adapter
	{
		private readonly List<Post> newsFeed;

		public event EventHandler<int> ItemClick;

		public NewsAdapter(List<Post> list)
		{
			this.newsFeed = list;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var newsViewHolder = (NewsViewHolder)holder;
			newsViewHolder.NewsText.Text = newsFeed[position].Text;
			newsViewHolder.NewsDate.Text = DateTimeOffset.FromUnixTimeSeconds(long.Parse(newsFeed[position].Date)).LocalDateTime.ToString();
			if (newsFeed[position].Img != null)
			{
				newsViewHolder.NewsImg.Visibility = ViewStates.Visible;
				Glide.With(newsViewHolder.NewsImg.Context).Load(newsFeed[position].Img).FitCenter()
					 .Placeholder(Resource.Drawable.empty)
					 .DiskCacheStrategy(DiskCacheStrategy.All)
					 .Into(newsViewHolder.NewsImg);
			}
			else {
				newsViewHolder.NewsImg.Visibility = ViewStates.Gone;
			}
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.news_item, parent, false);
			return new NewsViewHolder(layout, OnItemClick);
		}

		public override int ItemCount
		{
			get { return newsFeed.Count; }
		}

		void OnItemClick(int position)
		{
			if (ItemClick != null)
				ItemClick(this, position);
		}

	}
}
