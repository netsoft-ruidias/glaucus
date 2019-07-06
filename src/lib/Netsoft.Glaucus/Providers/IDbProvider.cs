// <copyright file="IDbProvider.cs" company="Netsoft">
// Copyright (c) Netsoft. All rights reserved.
// </copyright>

namespace Netsoft.Glaucus.Providers
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;

	public interface IDbProvider : IDisposable
	{
		bool IsTransaction { get; }

		bool HasTable(string tableName);

		bool HasField(string tableName, string fieldName);

		DbFields GetQuerySchema(string querySQL);

		DbSelect Select(string tableName, IEnumerable<string> fields, string whereCondition = "", string orderBy = "");

		DbSelect Select(string query, DbParameters parameters);

		object Insert(string tableName, DbFields values, string uuidField = null);

		int Update(string tableName, DbFields values, string whereCondition, DbParameters parameters = null);

		int Delete(string tableName, string whereCondition, DbParameters parameters = null);

		Hashtable ExecuteSP(string spName, DbParameters parameters, params string[] returnParams);

		Hashtable ExecuteSP(string spName, DbParameters parameters, ref DataTable dataTable, params string[] returnParams);

		int ExecuteNonQuery(string query, DbParameters parameters = null);

		object ExecuteScalar(string query, DbParameters parameters = null);

		void BeginTransaction();

		void CommitTransaction();

		void RollbackTransaction();

		IDisposable WithTransactions();
	}
}
