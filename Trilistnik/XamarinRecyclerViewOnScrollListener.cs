using System;
using Android.Support.V7.Widget;

namespace Trilistnik
{
	public class XamarinRecyclerViewOnScrollListener : RecyclerView.OnScrollListener
	{
		public delegate void LoadMoreEventHandler(object sender, EventArgs e);
		public event LoadMoreEventHandler LoadMoreEvent;
		private bool isLoading = false;
		private int previousTotal;
		private int visibleThreshold = 2;

		private LinearLayoutManager layoutManager;

		public XamarinRecyclerViewOnScrollListener(LinearLayoutManager layoutManager)
		{
			this.layoutManager = layoutManager;
		}

		public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
		{

			base.OnScrolled(recyclerView, dx, dy);

			var visibleItemCount = recyclerView.ChildCount;
			var totalItemCount = recyclerView.GetAdapter().ItemCount;
			var pastVisiblesItems = layoutManager.FindFirstVisibleItemPosition();

			if (!isLoading && (visibleItemCount + pastVisiblesItems + visibleThreshold) >= totalItemCount)
			{
				isLoading = true;
				previousTotal = totalItemCount;
				LoadMoreEvent(this, null);
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
