// <copyright file="DbEngine.cs" company="Netsoft">
// Copyright (c) Netsoft. All rights reserved.
// </copyright>

namespace Netsoft.Glaucus
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using Netsoft.Glaucus.Providers;

	public class DbEngine : IDisposable
	{
		private readonly IDbProvider dbProvider = null;

		public DbEngine(IDbProvider provider, string applicationName = "")
		{
			this.dbProvider = provider;

			if (!string.IsNullOrWhiteSpace(applicationName) && this.dbProvider is DbProviderBase)
			{
				var connectionString = (this.dbProvider as DbProviderBase).Connection.ConnectionString;

				if (!connectionString.Trim().EndsWith(";"))
				{
					connectionString += ";";
				}

				if (!connectionString.Contains("Application Name"))
				{
					connectionString += string.Format("Application Name={0};", applicationName);
				}

				(this.dbProvider as DbProviderBase).Connection.ConnectionString = connectionString;
			}
		}

		public bool IsInTransaction
		{
			get
			{
				return this.dbProvider == null
					? false
					: this.dbProvider.IsTransaction;
			}
		}

		#region Methods

		/// <summary>
		/// Check if table exists in database
		/// </summary>
		/// <param name="tableName">Table name</param>
		/// <returns>true is table exists, false otherwise</returns>
		public bool HasTable(string tableName) =>
			this.dbProvider.HasTable(tableName);

		/// <summary>
		/// Check if table has field
		/// </summary>
		/// <param name="tableName">Table name</param>
		/// <param name="fieldName">Field name</param>
		/// <returns>true is field exists, false otherwise</returns>
		public bool HasField(string tableName, string fieldName) =>
			this.dbProvider.HasField(tableName, fieldName);

		public DbFields GetQuerySchema(string querySQL) =>
			this.dbProvider.GetQuerySchema(querySQL);

		public Hashtable ExecuteSP(string spName, DbParameters parameters, params string[] returnParams) =>
			this.dbProvider.ExecuteSP(spName, parameters, returnParams);

		public Hashtable ExecuteSP(string spName, DbParameters parameters, ref DataTable dataTable, params string[] returnParams) =>
			this.dbProvider.ExecuteSP(spName, parameters, ref dataTable, returnParams);

		public int ExecuteNonQuery(string query, DbParameters parameters = null) =>
			this.dbProvider.ExecuteNonQuery(query, parameters);

		public object ExecuteScalar(string query, DbParameters parameters = null) =>
			this.dbProvider.ExecuteScalar(query, parameters);

		public DbSelect Select(string tableName, IEnumerable<string> fields, string whereCondition = "", string orderBy = "") =>
			this.dbProvider.Select(tableName, fields, whereCondition, orderBy);

		public DbSelect Select(string query) =>
			this.dbProvider.Select(query, parameters: null);

		public DbSelect Select(string query, DbParameters parameters) =>
			this.dbProvider.Select(query, parameters);

		public DataTable Query(string query) =>
			this.Query(query, parameters: null);

		public DataTable Query(string query, DbParameters parameters) =>
			this.dbProvider.Select(query, parameters).ToDataTable();

		public object Insert(string tableName, DbFields values) =>
			this.dbProvider.Insert(tableName, values);

		public object Insert<T>(string tableName, T obj, string uuidField, params string[] ignoreFields)
		{
			var dbFields = new DbFields();

			foreach (var property in obj.GetType().GetProperties())
			{
				if (ignoreFields == null || !ignoreFields.Contains(property.Name, StringComparer.OrdinalIgnoreCase))
				{
					var value = property.GetValue(obj, null);
					if (value != null)
					{
						dbFields.Add(property.Name, value);
					}
				}
			}

			return this.dbProvider.Insert(
				tableName,
				dbFields,
				string.IsNullOrWhiteSpace(uuidField) ? null : uuidField);
		}

		public int Update(string tableName, DbFields values, string whereCondition) =>
			this.dbProvider.Update(tableName, values, whereCondition);

		public int Update(string tableName, DbFields values, string whereCondition, DbParameters parameters) =>
			this.dbProvider.Update(tableName, values, whereCondition, parameters);

		public int Update<T>(string tableName, T obj, string idField, params string[] ignoreFields)
		{
			var dbFields = new DbFields();

			foreach (var property in obj.GetType().GetProperties())
			{
				if (property.Name != idField && (ignoreFields == null || !ignoreFields.Contains(property.Name, StringComparer.OrdinalIgnoreCase)))
				{
					var value = property.GetValue(obj, null);
					if (value != null)
					{
						dbFields.Add(property.Name, value);
					}
				}
			}

			return this.dbProvider.Update(
				tableName,
				dbFields,
				$"{idField} = @{idField}",
				new DbParameters() { { idField, obj.GetType().GetProperty(idField).GetValue(obj, null) } });
		}

		public int Delete(string tableName, string whereCondition) =>
			this.dbProvider.Delete(tableName, whereCondition);

		public int Delete(string tableName, string whereCondition, DbParameters parameters) =>
			this.dbProvider.Delete(tableName, whereCondition, parameters);

		public IDisposable WithTransactions()
		{
			return this.dbProvider.WithTransactions();
		}

		public void RollbackTransaction()
		{
			this.dbProvider.RollbackTransaction();
		}

		#endregion

		public void Dispose()
		{
			this.dbProvider.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
