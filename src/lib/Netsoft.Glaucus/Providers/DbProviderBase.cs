// <copyright file="DbProviderBase.cs" company="Netsoft">
// Copyright (c) Netsoft. All rights reserved.
// </copyright>

namespace Netsoft.Glaucus.Providers
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.Common;
	using System.Linq;
	using System.Text.RegularExpressions;

	public abstract class DbProviderBase : IDbProvider, IDisposable
	{
		private static readonly object Locker = new object();
		private IDbTransaction transaction = null;
        private bool disposed = false;

        protected DbProviderBase()
		{ }

		public bool IsTransaction { get; private set; } = false;

		protected internal IDbConnection Connection { get; set; } = null;

		internal IDbCommand GetCommand()
		{
			var command = this.Connection.CreateCommand();

			if (this.transaction != null)
			{
				command.Transaction = this.transaction;
			}

			return command;
		}

		/// <summary>
		/// This method should be used only for SELECT queries
		/// </summary>
		/// <param name="query">The Select query</param>
		/// <param name="parameters">Select query parameters</param>
		/// <returns>Database Command</returns>
		internal IDbCommand GetCommand(string query, Dictionary<string, object> parameters)
		{
			var command = this.GetCommand();

			command.CommandText = query;

			command.Parameters?.Clear();
			command.AddParameters(parameters);

			return command;
		}

		internal void CloseConnection()
		{
			if (this.Connection.State == ConnectionState.Open && !this.IsTransaction)
			{
				this.Connection.Close();
			}
		}

		protected internal abstract DbDataAdapter GetDataAdapter();

		#region System Helpers

		public virtual bool HasTable(string tableName)
		{
			throw new NotImplementedException("Not implemented by provider");
		}

		public virtual bool HasField(string tableName, string fieldName)
		{
			throw new NotImplementedException("Not implemented by provider");
		}

		public virtual DbFields GetQuerySchema(string querySQL)
		{
			var result = new DbFields();

			var dbParameters = new DbParameters(
				GetQueryParameters(querySQL)
					.ToDictionary(x => x, x => (object)DBNull.Value));

			var reader = this.Select(querySQL, dbParameters).ToDataReader();
			var schemaTable = reader.GetSchemaTable();

			foreach (DataRow row in schemaTable.Rows)
			{
				string columnName = row.ItemArray[0].ToString().Length > 0
                    ? row.ItemArray[0].ToString()
                    : $"Column{schemaTable.Rows.IndexOf(row) + 1}";

				if (string.IsNullOrEmpty(columnName))
				{
					int columnIndex = 0;
					do
					{
						columnName = $"Column{++columnIndex}";
					}
					while (result.ContainsKey(columnName));
				}

				if (!result.ContainsKey(columnName))
				{
					result.Add(columnName, string.Empty);
				}
			}

			return result;
		}

		private static string[] GetQueryParameters(string sqlQuery)
		{
			var result = new List<string>();

			foreach (Match match in Regex.Matches(sqlQuery, @"@(\w*)"))
			{
				if (match.Success && !result.Contains(match.Groups[1].Value))
				{
					result.Add(match.Groups[1].Value);
				}
			}

			return result.ToArray();
		}

		#endregion

		public DbSelect Select(string tableName, IEnumerable<string> fields, string whereCondition = "", string orderBy = "")
		{
			return this.Select(
				string.Format(
					"SELECT {0} FROM [{1}] {2} {3}",
					string.Join(", ", fields.Select(x => $"[{x}]")),
					tableName,
					string.IsNullOrWhiteSpace(whereCondition) ? string.Empty : $"WHERE {whereCondition}",
					string.IsNullOrWhiteSpace(orderBy) ? string.Empty : $"ORDER By {orderBy}"),
				parameters: null);
		}

		public DbSelect Select(string query, DbParameters parameters)
		{
			return new DbSelect(this, query, parameters);
		}

		/// <summary>
		/// Insert values into a Database table
		/// </summary>
		/// <param name="tableName">Table Name</param>
		/// <param name="values">Name and Value pairs to insert into new row</param>
		/// <param name="uuidField">The UUID field name</param>
		/// <returns>Object query result</returns>
		public object Insert(string tableName, DbFields values, string uuidField = null)
		{
			/* NOTE! This code prevents SQL Injection */

			var command = this.GetCommand();
			command.CommandType = CommandType.Text;

			command.CommandText = string.Format(
				string.IsNullOrWhiteSpace(uuidField)
					? "INSERT INTO [{0}] ({1}) VALUES ({2}); SELECT @@IDENTITY"
					: "INSERT INTO [{0}] ({1}) OUTPUT inserted.[" + uuidField + "] VALUES ({2});",
				tableName,
				string.Join(", ", values.Select(x => $"[{x.Key}]")),
				string.Join(", ", values.Select(x => $"@{x.Key}")));

			command.AddParameters(values);

			if (this.Connection.State == ConnectionState.Closed || this.Connection.State == ConnectionState.Broken)
			{
				this.Connection.Open();
			}

			return command.ExecuteScalar();
		}

		#region Update

		public int Update(string tableName, DbFields values, string whereCondition, DbParameters parameters = null)
		{
			if (parameters == null)
			{
				parameters = new DbParameters();
			}

			if (!string.IsNullOrWhiteSpace(whereCondition) && !whereCondition.Trim().ToUpper().StartsWith("WHERE"))
			{
				whereCondition = $"WHERE {whereCondition}";
			}

			var queryParameters = new DbParameters(
				values.ToDictionary(x => x.Key, x => x.Value));

			queryParameters.Concat(
				parameters.ToDictionary(x => x.Key, x => x.Value));

			return this.ExecuteNonQuery(
				$"UPDATE [{tableName}] SET {string.Join(", ", values.Select(x => $"[{x.Key}]=@{x.Key}"))} {whereCondition}",
				queryParameters);
		}

		#endregion

		#region Delete

		public int Delete(string tableName, string whereCondition, DbParameters parameters = null)
		{
			if (parameters == null)
			{
				parameters = new DbParameters();
			}

			return this.ExecuteNonQuery(
				$"DELETE FROM {tableName} WHERE {whereCondition}",
				parameters);
		}

		#endregion

		public Hashtable ExecuteSP(string spName, DbParameters parameters, params string[] returnParams)
		{
			var result = new Hashtable();

			using (var command = this.GetCommand())
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandText = spName;

				if (parameters != null)
				{
                    foreach (var parameter in parameters)
					{
						var dbParameter = command.CreateParameter();
						dbParameter.ParameterName = parameter.Key;
						dbParameter.Value = parameter.Value;
						dbParameter.DbType = parameter.Value.GetDbType();
						command.Parameters.Add(dbParameter);
					}
				}

				if (returnParams != null)
				{
					foreach (var outputParam in returnParams)
					{
						var dbParameter = command.CreateParameter();
						dbParameter.ParameterName = outputParam;
						dbParameter.Direction = ParameterDirection.Output;
						dbParameter.Size = 255;
						dbParameter.Value = DBNull.Value;
						command.Parameters.Add(dbParameter);
					}
				}

				command.ExecuteNonQuery();

				for (int i = 0; i < command.Parameters.Count; i++)
				{
                    if (command.Parameters[i] is DbParameter dbParameter &&
                        dbParameter.Direction == ParameterDirection.Output)
                    {
                        result.Add(dbParameter.ParameterName, dbParameter.Value);
                    }
                }
            }

			return result;
		}

		public Hashtable ExecuteSP(string spName, DbParameters parameters, ref DataTable dataTable, params string[] returnParams)
		{
			var result = new Hashtable();

			using (var command = this.GetCommand())
			{
				command.CommandType = CommandType.StoredProcedure;
				command.CommandText = spName;

				if (parameters != null)
				{
					command.AddParameters(parameters);
				}

				if (returnParams != null)
				{
					foreach (var outputParam in returnParams)
					{
						IDbDataParameter dbParameter = command.CreateParameter();
						dbParameter.ParameterName = outputParam;
						dbParameter.Direction = ParameterDirection.Output;
						dbParameter.Size = 255;
						dbParameter.Value = DBNull.Value;
						command.Parameters.Add(dbParameter);
					}
				}

				using (var dataAdapter = this.GetDataAdapter())
				{
					dataAdapter.SelectCommand = command as DbCommand;

					dataTable.BeginLoadData();
					dataAdapter.Fill(dataTable);
					dataTable.EndLoadData();
				}

				for (int i = 0; i < command.Parameters.Count; i++)
				{
					if (command.Parameters[i] is DbParameter dbParameter &&
                        dbParameter.Direction == ParameterDirection.Output)
					{
                        result.Add(dbParameter.ParameterName, dbParameter.Value);
					}
				}
			}

			return result;
		}

		public int ExecuteNonQuery(string query, DbParameters parameters = null)
		{
			lock (Locker)
			{
				using (var command = this.GetCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = query;

					if (parameters != null)
					{
						command.AddParameters(parameters);
					}

					if (this.Connection.State == ConnectionState.Closed || this.Connection.State == ConnectionState.Broken)
					{
						this.Connection.Open();
					}

					return command.ExecuteNonQuery();
				}
			}
		}

		public object ExecuteScalar(string query, DbParameters parameters = null)
		{
			lock (Locker)
			{
				using (var command = this.GetCommand())
				{
					command.CommandType = CommandType.Text;
					command.CommandText = query;

					if (parameters != null)
					{
						command.AddParameters(parameters);
					}

					if (this.Connection.State == ConnectionState.Closed || this.Connection.State == ConnectionState.Broken)
					{
						this.Connection.Open();
					}

					return command.ExecuteScalar();
				}
			}
		}

		#region Transactions

		public void BeginTransaction()
		{
			this.transaction = this.Connection.BeginTransaction();
			this.IsTransaction = true;
		}

		public void CommitTransaction()
		{
			if (this.transaction != null && this.IsTransaction)
			{
				this.transaction.Commit();
				this.transaction = null;
				this.IsTransaction = false;

				if (this.Connection.State == ConnectionState.Open)
				{
					this.Connection.Close();
				}
			}
		}

		public void RollbackTransaction()
		{
			if (this.transaction != null && this.IsTransaction)
			{
				this.transaction.Rollback();
				this.transaction = null;
				this.IsTransaction = false;

				if (this.Connection.State == ConnectionState.Open)
				{
					this.Connection.Close();
				}
			}
		}

		public IDisposable WithTransactions()
		{
			return new DeferredTransaction(this);
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                if (this.Connection != null && this.Connection.State == ConnectionState.Open)
                {
                    if (this.IsTransaction)
                    {
                        this.RollbackTransaction();
                    }

                    this.Connection.Close();
                    this.Connection.Dispose();
                }
            }

            this.disposed = true;
        }

        #endregion
    }
}
