// <copyright file="DataExtensions.cs" company="Netsoft">
// Copyright (c) Netsoft. All rights reserved.
// </copyright>

namespace Netsoft.Glaucus
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;

	public static partial class DataExtensions
	{
		public static void AddParameter(this IDbCommand command, string parameterName, DbType dbType, object value)
		{
			var result = command.CreateParameter();
			result.ParameterName = parameterName;

			result.DbType = dbType;
			result.Value = value;

			command.Parameters.Add(result);
		}

		public static void AddParameters(this IDbCommand command, IDictionary<string, object> parameters)
		{
			if (command != null && parameters != null && parameters.Any())
			{
				foreach (var parameter in parameters)
				{
					if (command.CommandText.Contains("@" + parameter.Key))
					{
						command.AddParameter(
							parameter.Key,
							parameter.Value == null ? DbType.String : parameter.Value.GetDbType(),
							parameter.Value ?? DBNull.Value);
					}
				}
			}
		}

		public static DataTable OrderBy(this DataTable dataTable, string columnName)
		{
			var result = dataTable.Clone();

			foreach (var row in dataTable.Select(null, columnName))
			{
				DataRow newRow = result.NewRow();
				newRow.ItemArray = row.ItemArray;
				result.Rows.Add(newRow);
			}

			return result;
		}

		public static DataRow FirstOrDefault(this DataTable dataTable)
		{
			if (dataTable == null || dataTable.Rows == null || dataTable.Rows.Count == 0)
			{
				return null;
			}

			return dataTable.Rows[0];
		}

		public static DataRow LastOrDefault(this DataTable dataTable)
		{
			if (dataTable == null || dataTable.Rows == null || dataTable.Rows.Count == 0)
			{
				return null;
			}

			return dataTable.Rows[dataTable.Rows.Count - 1];
		}

		public static T ToEntity<T>(this DataRow row)
			where T : new()
		{
			var item = new T();
			var itemType = item.GetType();

			foreach (DataColumn column in row.Table.Columns)
			{
				var itemProperty = itemType.GetProperty(column.ColumnName);

				if (itemProperty == null)
				{
					// NotFound By Name, try by Attribute!
					itemProperty = itemType
						.GetProperties()
						.Where(x => x.GetCustomAttributes(typeof(ColumnAttribute), true).Where(y => ((ColumnAttribute)y).Name.Equals(column.ColumnName, StringComparison.CurrentCultureIgnoreCase)).Any())
						.FirstOrDefault();      // If more than one, get only the first
				}

				if (itemProperty != null)
				{
					itemProperty.SetValue(item, GetValue(row[column.ColumnName], itemProperty.PropertyType), null);
				}
			}

			return item;
		}

		// This is used only by 'ToEntity' method
		private static object GetValue(object original, Type objectType)
		{
			if (original == DBNull.Value)
			{
				return (objectType == typeof(string))
					? string.Empty
					: Activator.CreateInstance(objectType);
			}

			return original;
		}
	}
}
