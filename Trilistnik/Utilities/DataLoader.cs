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
	public class DataLoader
	{
		private const string odintsovoCode = "182209";
		private const string belorusCode = "198230";
		private const string begovCode = "198211";
		private const string kursCode = "191509";

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
					string apiUrlFinal = offset != 0 ? ApiKeys.vkapiurl + "&offset=" + offset.ToString() : ApiKeys.vkapiurl;
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
		public async static Task<IEnumerable<TrainInfo>> GetTransportData(string from, string to, string date)
		{
			try
			{
				using (var w = new HttpClient())
				{
					//from = 182209 & to = 198230 & page = 1 & date = 2017 - 03 - 02"
					var yandexFinalurl = ApiKeys.yandexapiurl + "from=" + GetCode(from) + "&to=" + GetCode(to) + "&date=" + date;
					var resp = await w.GetStringAsync(yandexFinalurl);
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
			return newsFeedLocal;
		}

		private static string GetCode(string dest)
		{
			switch (dest)
			{
				case "Белорусская": return belorusCode;
				case "Одинцово": return odintsovoCode;
				case "Курская": return kursCode;
				case "Беговая": return begovCode;

			}
			return String.Empty;
		}
	}
}
