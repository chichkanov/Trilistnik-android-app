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
		public event EventHandler<int> ItemClick;

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
			     .Into(viewHolder.Img);
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var layout = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.item_goods, parent, false);
			return new GoodsViewHolder(layout, OnClick);
		}

		private void OnClick(int position)
		{
			if (ItemClick != null)
			{
				ItemClick(this, position);
			}
		}

		public override int ItemCount
		{
			get { return dataset.Count; }
		}
	}
}
