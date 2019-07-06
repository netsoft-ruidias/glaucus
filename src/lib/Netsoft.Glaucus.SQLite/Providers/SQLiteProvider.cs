namespace Netsoft.Glaucus.Providers
{
	using System.Data.Common;
	using System.Linq;
	using Microsoft.Data.Sqlite;

	public class SQLiteProvider : DbProviderBase
	{
		public SQLiteProvider(string databaseName, int version = 3)
		{
			this.Connection = new SqliteConnection(
				new SqliteConnectionStringBuilder
				{
					DataSource = databaseName
				}
				.ConnectionString);
		}

		public void CreateTable(string tablename, DbFields fields)
		{
			var fieldsTxt = fields.Select(x => $"{x.Key} {x.Value}");

			this.ExecuteNonQuery(
				$"CREATE TABLE {tablename} ({string.Join(",", fieldsTxt)})");
		}

		protected override DbDataAdapter GetDataAdapter()
		{
			return new SqliteDataAdapter();
		}
	}
}
