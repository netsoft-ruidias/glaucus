namespace Netsoft.Glaucus.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Text;
    using MySql.Data.MySqlClient;

    public class MySQLProvider : DbProviderBase
    {
        public MySQLProvider(string connectionString)
        {
            this.Connection = new MySqlConnection(connectionString);
        }

        public MySQLProvider(string server, string databaseName, string userID, string password)
        {
            var builder = new MySqlConnectionStringBuilder
            {
                Server = server,
                Database = databaseName,
                UserID = userID,
                Password = password,
                SslMode = MySqlSslMode.Preferred
            };

            this.Connection = new MySqlConnection(builder.ConnectionString);
        }


        public override bool HasTable(string tableName)
        {
            var result = this.ExecuteScalar(@"
				IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME=@tableName) 
				SELECT 1 ELSE SELECT 0",
                new DbParameters
                {
                    { "tableName", tableName }
                });

            return Convert.ToInt32(result) == 1;
        }

        public override bool HasField(string tableName, string fieldName)
        {
            var result = this.ExecuteScalar(@"
				IF EXISTS (SELECT * FROM sys.columns  WHERE Name = @fieldName AND Object_ID = Object_ID(@tableName)) 
				SELECT 1 ELSE SELECT 0",
                new DbParameters
                {
                    { "tableName", tableName },
                    { "fieldName", fieldName }
                });

            return Convert.ToInt32(result) == 1;
        }

        public bool DropField(string tableName, string fieldName)
        {
            this.ExecuteScalar(
                @"DECLARE @sql NVARCHAR(MAX)
						WHILE 1=1
						BEGIN
							SELECT TOP 1 @sql = N'alter table @tableName drop constraint ['+dc.NAME+N']'
							from sys.default_constraints dc
							JOIN sys.columns c
								ON c.default_object_id = dc.object_id
							WHERE 
								dc.parent_object_id = OBJECT_ID(@tableName)
							AND c.name = @fieldName
							IF @@ROWCOUNT = 0 BREAK
							EXEC (@sql)
						END",
                new DbParameters
                {
                    { "tableName", tableName },
                    { "fieldName", fieldName }
                });

            // ToDo: fix this code to allow parameters
            var result = this.ExecuteScalar($"ALTER TABLE {tableName} DROP COLUMN {fieldName}");

            return Convert.ToInt32(result) == 1;
        }

        protected override DbDataAdapter GetDataAdapter()
        {
            return new MySqlDataAdapter();
        }
    }
}
