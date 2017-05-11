using System;
namespace Trilistnik
{
	public class GoodsItem
	{
		public GoodsItem(String title, String desc, String date, String img, String id)
		{
			Date = date;
			Title = title;
			Img = img;
			Desc = desc;
			Id = id;
		}

		public string Date { get; set; }
		public string Title { get; set; }
		public string Desc { get; set; }
		public string Img { get; set; }
		public string Id { get; set; }

	}
}
