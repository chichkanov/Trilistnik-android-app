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
		private const string vkapiurl = "https://api.vk.com/method/wall.get?v=5.50&count=20&owner_id=-33374477&filter=owner";
		private const string yandexapiurl = "https://api.rasp.yandex.net/v1.0/search/?apikey=723f283e-9bdd-4b1e-92ac-9cda196d1e70&format=json&lang=ru&system=esr&transport_types=suburban&from=182209&to=198230&page=1&date=2017-03-02";

		/// <summary>
		/// Get news feed from vk api
		/// </summary>
		/// <returns>The data</returns>
		/// <param name="offset">Offset</param>
		public async static Task<IEnumerable<Post>> GetNewsData(int offset = 0)
		{
			try
			{
				using (var w = new HttpClient())
				{
					string apiUrlFinal = offset != 0 ? vkapiurl + "&offset=" + offset.ToString() : vkapiurl;
					var resp = await w.GetStringAsync(apiUrlFinal);
					JObject json = JObject.Parse(resp);
					if (offset == 0) NewsCache.CreateNewsData(json.ToString());
					return ParseNewsData(json);
				}
			}
			catch (System.Net.WebException)
			{
				return null;
			}
		}

		/// <summary>
		/// Parse news posts from data
		/// </summary>
		/// <returns>The data.</returns>
		/// <param name="json">Json.</param>
		public static IEnumerable<Post> ParseNewsData(JObject json)
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

		/// <summary>
		/// Get transport feed from Yandex api
		/// </summary>
		/// <returns>The data</returns>
		/// <param name="offset">Offset</param>
		public async static Task<IEnumerable<TrainInfo>> GetTransportData()
		{
			try
			{
				using (var w = new HttpClient())
				{
					var resp = await w.GetStringAsync(yandexapiurl);
					JObject json = JObject.Parse(resp);
					//if (offset == 0) NewsCache.CreateNewsData(json.ToString());
					return ParseTransportData(json);
				}
			}
			catch (System.Net.WebException)
			{
				return null;
			}
		}

		/// <summary>
		/// Parse transport info from data
		/// </summary>
		/// <returns>The data.</returns>
		/// <param name="json">Json.</param>
		public static IEnumerable<TrainInfo> ParseTransportData(JObject json)
		{
			JArray trains = (JArray)json["threads"];

			var newsFeedLocal = from train in trains
								select new TrainInfo
								{
									Departure = train["departure"].ToString(),
				Arrival = train["arrival"].ToString(),
				Stops = train["stops"].ToString(),
				Title = train["thread"]["title"].ToString(),
				Duration = train["duration"].ToString()
								};
			Console.WriteLine(trains[1]["thread"]["title"]);
			return newsFeedLocal;
		}
	}
}
