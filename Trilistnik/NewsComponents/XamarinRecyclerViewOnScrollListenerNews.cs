﻿using System;
using Android.Support.V7.Widget;
using Android.Widget;

namespace Trilistnik
{
	public class XamarinRecyclerViewOnScrollListenerNews : RecyclerView.OnScrollListener
	{
		public delegate void LoadMoreEventHandler(object sender, EventArgs e);
		public event LoadMoreEventHandler LoadMoreEvent;
		private bool isLoading = false;
		public static bool isNotifyShowing = false;
		private int previousTotal;
		private int visibleThreshold = 2;

		private LinearLayoutManager layoutManager;

		public XamarinRecyclerViewOnScrollListenerNews(LinearLayoutManager layoutManager)
		{
			this.layoutManager = layoutManager;
		}

		public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
		{

			base.OnScrolled(recyclerView, dx, dy);

			var visibleItemCount = recyclerView.ChildCount;
			var totalItemCount = recyclerView.GetAdapter().ItemCount;
			var pastVisiblesItems = layoutManager.FindFirstVisibleItemPosition();

			if (!isNotifyShowing && !isLoading && (visibleItemCount + pastVisiblesItems + visibleThreshold) >= totalItemCount)
			{
				if (MainActivity.isOnline)
				{
					isLoading = true;
					previousTotal = totalItemCount;
					LoadMoreEvent(this, null);
				}
				else {
					if (!isNotifyShowing)
					{
						Toast.MakeText(MainActivity.context, "Для загрузки новостей ребуется интернет соединение", ToastLength.Short).Show();
						isNotifyShowing = true;
					}
				}
			}

			if (isLoading)
			{
				if (totalItemCount > previousTotal)
				{
					isLoading = false;
					previousTotal = totalItemCount;
				}
			}
		}
	}
}
