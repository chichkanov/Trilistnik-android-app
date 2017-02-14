using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;

namespace Trilistnik
{
	public class Database
	{
		private string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

		public Database()
		{
		}

		public bool CreateDatabase()
		{
			try
			{
				var connection = new SQLiteAsyncConnection(System.IO.Path.Combine(path, "Database.db"));
				{
					connection.CreateTableAsync<Post>();
					return true;
				}
			}
			catch (SQLiteException ex)
			{
				return false;
			}
		}

		public bool InsertNewsPost(Post post)
		{
			try
			{
				var connection = new SQLiteAsyncConnection(System.IO.Path.Combine(path, "Database.db"));
				{
					connection.InsertAsync(post);
					return true;
				}
			}
			catch (SQLiteException ex)
			{
				return false;
			}
		}

		public Task<List<Post>> SelectNewsFeed()
		{
			try
			{
				var connection = new SQLiteAsyncConnection(System.IO.Path.Combine(path, "Database.db"));
				{
					return connection.Table<Post>().ToListAsync();
				}
			}
			catch (SQLiteException ex)
			{
				return null;
			}
		}
	}
}
