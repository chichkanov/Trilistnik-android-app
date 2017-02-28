using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Trilistnik
{
	public class JsonDataLoader
	{
		private const string apiurl = "https://api.vk.com/method/wall.get?v=5.50&count=20&owner_id=-33374477&filter=owner";

		/// <summary>
		/// Get newsFeed from api
		/// </summary>
		/// <returns>The data</returns>
		/// <param name="offset">Offset</param>
		public async static Task<IEnumerable<Post>> GetData(int offset = 0)
		{
			using (var w = new HttpClient())
			{
				string apiUrlFinal = offset != 0 ? apiurl + "&offset=" + offset.ToString() : apiurl;
				var resp = await w.GetStringAsync(apiUrlFinal);
				JObject json = JObject.Parse(resp);
				if(offset == 0) NewsCache.CreateNewsData(json.ToString());
				return ParseData(json);
			}
		}

		/// <summary>
		/// Parse news posts from data
		/// </summary>
		/// <returns>The data.</returns>
		/// <param name="json">Json.</param>
		public static IEnumerable<Post> ParseData(JObject json)
		{
			JArray posts = (JArray)json["response"]["items"];

			var newsFeedLocal = from post in posts
								where post["text"].ToString() != String.Empty
								select new Post
								{
									Text = post["text"].ToString(),
									Date = post["date"].ToString(),
									Img = post["attachments"]?[0]?["photo"]?["photo_604"].ToString()
								};
			return newsFeedLocal;
		}
	}
}
