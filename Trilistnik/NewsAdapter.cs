using System;
using System.Collections.Generic;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Com.Bumptech.Glide;

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
			newsViewHolder.NewsDate.Text = DateTimeOffset.FromUnixTimeSeconds(long.Parse(newsFeed[position].Date)).DateTime.ToString();
			// Placeholder : 
			if (newsFeed[position].Img != null)
			{
				Glide.With(newsViewHolder.NewsImg.Context).Load(newsFeed[position].Img)
					 .Placeholder(Resource.Drawable.empty)
					 .Into(newsViewHolder.NewsImg);
			}
			else {
				newsViewHolder.NewsImg.SetImageDrawable(null);
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
