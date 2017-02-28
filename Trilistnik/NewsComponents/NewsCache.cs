using System;
using System.IO;
using Android.Content;

namespace Trilistnik
{
	public class NewsCache
	{
		// Name of news cache file
		private static readonly string fileName = "newsCache.txt";

		/// <summary>
		/// Put cache data into the file
		/// </summary>
		/// <param name="data">News data</param>
		public static void CreateNewsData(string data)
		{
			File.WriteAllText(PathToFile(), data);
		}

		/// <summary>
		/// Get all news data from file
		/// </summary>
		/// <returns>News data</returns>
		public static string ReadNewsData() => File.ReadAllText(PathToFile());

		/// <summary>
		/// Delete news cache
		/// </summary>
		public static void DeleteNewsData()
		{
			File.Delete(PathToFile());
		}

		/// <summary>
		/// Path to news cache file
		/// </summary>
		/// <returns>Path to the file</returns>
		private static string PathToFile() => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), fileName);

		/// <summary>
		/// Check if cache file already exist
		/// </summary>
		/// <returns><c>true</c>exist<c>false</c>not exist</returns>
		public static bool IsExist() => File.Exists(PathToFile());

	}
}
