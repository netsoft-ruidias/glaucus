// <copyright file="DbTypeExtensions.cs" company="Netsoft">
// Copyright (c) Netsoft. All rights reserved.
// </copyright>

namespace Netsoft.Glaucus
{
	using System;
	using System.Data;

	public static partial class DbTypeExtensions
	{
		public static DbType GetDbType(this object value)
		{
			return value
				.GetType()
				.ToDbType(value);
		}

		public static DbType ToDbType(this TypeCode typeCode, object value)
		{
			return ToDbType(
				typeCode,
				value == null ? string.Empty : value.ToString());
		}

		public static DbType ToDbType(this TypeCode typeCode, string value)
		{
			DbType dbType = DbType.String;
			switch (typeCode)
			{
				case TypeCode.Boolean: dbType = DbType.Boolean; break;
				case TypeCode.Byte: dbType = DbType.Byte; break;

				case TypeCode.Char: dbType = DbType.String; break;

				case TypeCode.DateTime: dbType = DbType.DateTime; break;
				case TypeCode.Decimal: dbType = DbType.Decimal; break;
				case TypeCode.Double: dbType = DbType.Double; break;

				case TypeCode.Int16:
				case TypeCode.UInt16: dbType = DbType.Int16; break;
				case TypeCode.Int32:
				case TypeCode.UInt32: dbType = DbType.Int32; break;
				case TypeCode.Int64:
				case TypeCode.UInt64: dbType = DbType.Int64; break;

				case TypeCode.SByte: dbType = DbType.Byte; break;
				case TypeCode.Single: dbType = DbType.Int16; break;
				case TypeCode.String: dbType = DbType.String; break;

				case TypeCode.Empty:
				case TypeCode.DBNull:
				case TypeCode.Object:
				default:
					dbType = DbType.String;
					break;
			}

			return dbType;
		}

		public static DbType ToDbType(this Type type, object value)
		{
			if (type == typeof(Guid))
			{
				return DbType.Guid;
			}

			if (type == typeof(byte[]))
			{
				return DbType.Binary;
			}

			return ToDbType(
				Type.GetTypeCode(type),
				value == null ? string.Empty : value.ToString());
		}
	}
}
