// <copyright file="DbSelect.cs" company="Netsoft">
// Copyright (c) Netsoft. All rights reserved.
// </copyright>

namespace Netsoft.Glaucus
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.Common;
	using System.Linq;
	using Netsoft.Glaucus.Providers;

	public sealed class DbSelect
	{
		private const string STREXCEPTION = "Invalid connection string or query!";

		private readonly string query = string.Empty;
		private readonly DbProviderBase provider = null;
		private readonly DbParameters parameters = null;

		internal DbSelect(IDbProvider provider, string query, DbParameters parameters)
		{
			this.provider = (DbProviderBase)provider;
			this.query = query;
			this.parameters = parameters;
		}

		public DataTable ToDataTable()
		{
			var result = new DataTable();

			if (!string.IsNullOrWhiteSpace(this.query))
			{
				using (var dataAdapter = this.provider.GetDataAdapter())
				{
					using (var command = this.provider.GetCommand(this.query, this.parameters))
					{
						/* http://msdn.microsoft.com/en-us/library/ms971481.aspx
						 * The Fill and Update methods, of the DataAdapter, automatically open the connection specified for the related command property if it is closed.
						 * If the Fill or Update method open the connection, Fill or Update will close it when the operation is complete.
						 */

						dataAdapter.SelectCommand = (DbCommand)command;

						result.BeginLoadData();
						dataAdapter.Fill(result);
						result.EndLoadData();
					}
				}
			}

			return result;
		}

		public DataTable ToDataTable(string sort) =>
			this.ToDataTable().OrderBy(sort);

		public DataSet ToDataSet(string tableName, DataSet dataSet = null)
		{
			if (!string.IsNullOrWhiteSpace(this.query))
			{
				if (dataSet == null)
				{
					dataSet = new DataSet();
				}

				using (var dataAdapter = this.provider.GetDataAdapter())
				{
					try
					{
						var command = this.provider.GetCommand(this.query, this.parameters);

						dataAdapter.SelectCommand = (DbCommand)command;

						// fill datatable
						dataSet.EnforceConstraints = false;
						dataAdapter.Fill(dataSet, tableName);
						dataSet.EnforceConstraints = true;

						this.provider.CloseConnection();

						return dataSet;
					}
					catch (Exception ex)
					{
						/* TODO! implement eventHandler for log exceptions */
						throw new Exception(STREXCEPTION, ex);
					}
				}
			}
			else
			{
				throw new Exception(STREXCEPTION);
			}
		}

		public List<T> ToList<T>()
			 where T : new()
		{
			return this.ToDataTable()
				.AsEnumerable()
				.Select(row => row.ToEntity<T>())
				.ToList<T>();
		}

		public DbDataReader ToDataReader() =>
			this.Get<DbDataReader>();

		public T ToEntity<T>()
			where T : new() => this.ToList<T>().FirstOrDefault();

		/// <summary>
		/// Get's (read) a value from a Scalar command
		/// </summary>
		/// <typeparam name="T">return desirable type</typeparam>
		/// <returns><see cref="T"/></returns>
		public T Get<T>()
		{
			if (!string.IsNullOrWhiteSpace(this.query))
			{
				object result = null;

				if (this.provider.Connection.State == ConnectionState.Closed || this.provider.Connection.State == ConnectionState.Broken)
				{
					this.provider.Connection.Open();
				}

				using (var command = this.provider.GetCommand(this.query, this.parameters))
				{
					if (typeof(T).GetInterfaces().Contains(typeof(IDataReader)))
					{
						result = command.ExecuteReader();
						return (T)result;
					}

					result = command.ExecuteScalar();
					this.provider.CloseConnection();
					return result == null || DBNull.Value.Equals(result)
						? default(T)
						: (T)System.Convert.ChangeType(result, typeof(T));
				}
			}
			else
			{
				throw new Exception(STREXCEPTION);
			}
		}

		public override string ToString() =>
			this.Get<string>();
	}
}
