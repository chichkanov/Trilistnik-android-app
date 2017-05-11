using System;
using System.Collections.Generic;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Com.Bumptech.Glide;
using Com.Bumptech.Glide.Load.Engine;

namespace Trilistnik
{
	public class GoodsAdapter : RecyclerView.Adapter
	{
		private readonly List<GoodsItem> dataset;

		public GoodsAdapter(List<GoodsItem> list)
		{
			this.dataset = list;
		}

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			var viewHolder = (GoodsViewHolder)holder;
			viewHolder.Title.Text = dataset[position].Title;
			viewHolder.Desc.Text = dataset[position].Desc;
			viewHolder.Date.Text = dataset[position].Date;
			Glide.With(viewHolder.Img.Context).Load(dataset[position].Img)
				 .DiskCacheStrategy(DiskCacheStrategy.All)
				 .Override(100, 100)
			     .Into(viewHolder.Img);
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_goods, parent, false);
			return new GoodsViewHolder(layout);
		}

		public override int ItemCount
		{
			get { return dataset.Count; }
		}
	}
}
