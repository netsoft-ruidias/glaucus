using System;
using System.Collections;
using System.Data;
using System.Linq;
using AutoFixture;
using Moq;
using Netsoft.Glaucus.Moq.Tests;
using Netsoft.Glaucus.Providers;
using Xunit;

namespace Netsoft.Glaucus.Tests
{
	public class DbEngineTest
	{
		private readonly DbEngine target;
		private readonly Fixture fixture = new Fixture();
		private readonly Mock<IDbProvider> dbProviderMock = null;

		public DbEngineTest()
		{
			this.dbProviderMock = new Mock<IDbProvider>();

			this.target = new DbEngine(
				this.dbProviderMock.Object,
				"ApplicarionTest");
		}

		#region HasTable

		[Fact]
		public void HasTable_Tablename_Exists()
		{
			// Arranje
			var tableName = this.fixture.Create<string>();

			this.dbProviderMock
				.Setup(x => x.HasTable(It.IsAny<string>()))
				.Returns(true);

			// Act
			var result = this.target.HasTable(tableName);

			// Assert
			dbProviderMock.Verify(x => x.HasTable(tableName), Times.Once);
			Assert.True(result);
		}

		[Fact]
		public void HasTable_Tablename_DoesNotExists()
		{
			// Arranje
			var tableName = this.fixture.Create<string>();

			this.dbProviderMock
				.Setup(x => x.HasTable(It.IsAny<string>()))
				.Returns(false);

			// Act
			var result = this.target.HasTable(tableName);

			// Assert
			dbProviderMock.Verify(x => x.HasTable(tableName), Times.Once);
			Assert.False(result);
		}

		[Fact]
		public void HasTable_Tablename_NotImplemented()
		{
			// Arranje
			var tableName = this.fixture.Create<string>();

			this.dbProviderMock
				.Setup(x => x.HasTable(It.IsAny<string>()))
				.Throws<NotImplementedException>();

			// Act		
			Action act = () => this.target.HasTable(tableName);

			// Assert
			Assert.Throws<NotImplementedException>(act);
		}

		#endregion HasTable

		#region HasField

		[Fact]
		public void HasField_TablenameFieldName_Exists()
		{
			// Arranje
			var tableName = this.fixture.Create<string>();
			var fieldName = this.fixture.Create<string>();

			this.dbProviderMock
				.Setup(x => x.HasField(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(true);

			// Act
			var result = this.target.HasField(tableName, fieldName);

			// Assert
			dbProviderMock.Verify(x => x.HasField(tableName, fieldName), Times.Once);
			Assert.True(result);
		}

		[Fact]
		public void HasField_TablenameFieldName_DoesNotExists()
		{
			// Arranje
			var tableName = this.fixture.Create<string>();
			var fieldName = this.fixture.Create<string>();

			this.dbProviderMock
				.Setup(x => x.HasField(It.IsAny<string>(), It.IsAny<string>()))
				.Returns(false);

			// Act
			var result = this.target.HasField(tableName, fieldName);

			// Assert
			dbProviderMock.Verify(x => x.HasField(tableName, fieldName), Times.Once);
			Assert.False(result);
		}

		[Fact]
		public void HasField_TablenameFieldName_NotImplemented()
		{
			// Arranje
			var tableName = this.fixture.Create<string>();
			var fieldName = this.fixture.Create<string>();

			this.dbProviderMock
				.Setup(x => x.HasField(It.IsAny<string>(), It.IsAny<string>()))
				.Throws<NotImplementedException>();

			// Act
			Action act = () => this.target.HasField(tableName, fieldName);

			// Assert
			Assert.Throws<NotImplementedException>(act);
		}

		#endregion HasField

		#region GetQuerySchema

		[Fact]
		public void GetQuerySchema_querySQL_Success()
		{
			// Arranje
			var querySQL = this.fixture.Create<string>();
			var expected = this.fixture.Build<DbFields>()
				.Do(x =>
				{
					x.Add(this.fixture.Create<string>(), this.fixture.Create<object>());
					x.Add(this.fixture.Create<string>(), this.fixture.Create<object>());
					x.Add(this.fixture.Create<string>(), this.fixture.Create<object>());
				})
				.Create();

			this.dbProviderMock
				.Setup(x => x.GetQuerySchema(It.IsAny<string>()))
				.Returns(expected);

			// Act
			var result = this.target.GetQuerySchema(querySQL);

			// Assert
			dbProviderMock.Verify(x => x.GetQuerySchema(querySQL), Times.Once);
			Assert.True(result.Count == 3);
			Assert.Equal(expected, result);
		}

		#endregion

		#region ExecuteSP

		[Fact]
		public void ExecuteSP_Parameters_Success()
		{
			// Arranje
			var spName = this.fixture.Create<string>();
			var parameters = this.fixture.Create<DbParameters>();
			var returnParams = this.fixture.CreateMany<string>().ToArray();

			var expected = this.fixture.Create<Hashtable>();

			this.dbProviderMock
				.Setup(x => x.ExecuteSP(It.IsAny<string>(), It.IsAny<DbParameters>(), It.IsAny<string[]>()))
				.Returns(expected);

			// Act
			var result = this.target.ExecuteSP(spName, parameters, returnParams);

			// Assert
			dbProviderMock.Verify(x => x.ExecuteSP(spName, parameters, returnParams), Times.Once);
			Assert.Equal(expected, result);
		}

		[Fact]
		public void ExecuteSP_ParametersDataTable_Success()
		{
			// Arranje
			var spName = this.fixture.Create<string>();
			var parameters = this.fixture.Create<DbParameters>();

			this.fixture.Register(() => new DataTable(
				this.fixture.Create<string>(),
				this.fixture.Create<string>()));

			var dataTable = this.fixture.Create<DataTable>();
			var returnParams = this.fixture.CreateMany<string>().ToArray();

			var expected = this.fixture.Create<Hashtable>();

			this.dbProviderMock
				.Setup(x => x.ExecuteSP(It.IsAny<string>(), It.IsAny<DbParameters>(), ref dataTable, It.IsAny<string[]>()))
				.Returns(expected);

			// Act
			var result = this.target.ExecuteSP(spName, parameters, ref dataTable, returnParams);

			// Assert
			dbProviderMock.Verify(x => x.ExecuteSP(spName, parameters, ref dataTable, returnParams), Times.Once);
			Assert.NotNull(dataTable);
			Assert.Equal(expected, result);
		}

		#endregion

		#region ExecuteNonQuery

		[Fact]
		public void ExecuteNonQuery_QueryParameters_Success()
		{
			// Arranje
			var query = this.fixture.Create<string>();
			var parameters = this.fixture.Create<DbParameters>();

			var expected = this.fixture.Create<int>();

			this.dbProviderMock
				.Setup(x => x.ExecuteNonQuery(It.IsAny<string>(), It.IsAny<DbParameters>()))
				.Returns(expected);

			// Act
			var result = this.target.ExecuteNonQuery(query, parameters);

			// Assert
			dbProviderMock.Verify(x => x.ExecuteNonQuery(query, parameters), Times.Once);
			Assert.Equal(expected, result);
		}

		#endregion ExecuteNonQuery

		#region ExecuteNonQuery

		[Fact]
		public void ExecuteScalar_QueryParameters_Success()
		{
			// Arranje
			var query = this.fixture.Create<string>();
			var parameters = this.fixture.Create<DbParameters>();

			var expected = this.fixture.Create<object>();

			this.dbProviderMock
				.Setup(x => x.ExecuteScalar(It.IsAny<string>(), It.IsAny<DbParameters>()))
				.Returns(expected);

			// Act
			var result = this.target.ExecuteScalar(query, parameters);

			// Assert
			dbProviderMock.Verify(x => x.ExecuteScalar(query, parameters), Times.Once);
			Assert.Equal(expected, result);
		}

		#endregion ExecuteNonQuery

		#region Insert

		[Fact]
		public void Insert_TableNameDbFields_Success()
		{
			// Arranje
			var tableName = this.fixture.Create<string>();
			var values = this.fixture.Build<DbFields>()
				.Do(x =>
				{
					x.Add(this.fixture.Create<string>(), this.fixture.Create<string>());
					x.Add(this.fixture.Create<string>(), this.fixture.Create<int>());
					x.Add(this.fixture.Create<string>(), this.fixture.Create<double>());
				})
				.Create();

			var expected = this.fixture.Create<object>();

			this.dbProviderMock
				.Setup(x => x.Insert(It.IsAny<string>(), It.IsAny<DbFields>(), null))
				.Returns(expected);

			// Act
			var result = this.target.Insert(tableName, values);

			// Assert
			dbProviderMock.Verify(x => x.Insert(tableName, values, null), Times.Once);
			Assert.Equal(expected, result);
		}

		[Fact]
		public void Insert_TableNameModelMoq_Success()
		{
			// Arranje
			DbFields dbFields = null;

			var tableName = this.fixture.Create<string>();
			var value = this.fixture.Create<ModelMoq>();
			var fieldCount = typeof(ModelMoq).GetProperties().Count();

			var expected = this.fixture.Create<object>();

			this.dbProviderMock
				.Setup(x => x.Insert(It.IsAny<string>(), It.IsAny<DbFields>(), It.IsAny<string>()))
				.Callback<string, DbFields, string>((param1, param2, param3) => dbFields = param2)
				.Returns(expected);

			// Act
			var result = this.target.Insert(tableName, value, string.Empty);

			// Assert
			dbProviderMock.Verify(x => x.Insert(tableName, It.IsAny<DbFields>(), null), Times.Once);
			Assert.True(dbFields.Count == fieldCount);
			Assert.Equal(expected, result);
		}

		[Fact]
		public void Insert_TableNameModelMoqIgnoreFields_Success()
		{
			// Arranje
			var idField = "Id";
			var ignoreField = new[] { "id", "version" };

			DbFields dbFields = null;

			var tableName = this.fixture.Create<string>();
			var value = this.fixture.Create<ModelMoq>();
			var fieldCount = typeof(ModelMoq).GetProperties().Count();

			var expected = this.fixture.Create<object>();

			this.dbProviderMock
				.Setup(x => x.Insert(It.IsAny<string>(), It.IsAny<DbFields>(), It.IsAny<string>()))
				.Callback<string, DbFields, string>((param1, param2, param3) => dbFields = param2)
				.Returns(expected);

			// Act
			var result = this.target.Insert(tableName, value, idField, ignoreField);

			// Assert
			dbProviderMock.Verify(x => x.Insert(tableName, It.IsAny<DbFields>(), idField), Times.Once);
			Assert.True(dbFields.Count == fieldCount - ignoreField.Count());
			Assert.Equal(expected, result);
		}

		#endregion Insert

		#region Update

		[Fact]
		public void Update_TableNameModelMoq_Success()
		{
			// Arranje
			var idField = "Id";

			DbFields dbFields = null;
			DbParameters dbParameters = null;

			var tableName = this.fixture.Create<string>();
			var value = this.fixture.Create<ModelMoq>();
			var propertyCount = typeof(ModelMoq).GetProperties().Count();

			var expected = this.fixture.Create<int>();

			this.dbProviderMock
				.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<DbFields>(), It.IsAny<string>(), It.IsAny<DbParameters>()))
				.Callback<string, DbFields, string, DbParameters>((param1, param2, param3, param4) =>
					{
						dbFields = param2;
						dbParameters = param4;
					})
				.Returns(expected);

			// Act
			var result = this.target.Update(tableName, value, idField);

			// Assert
			dbProviderMock.Verify(x => x.Update(tableName, dbFields, It.IsAny<string>(), It.IsAny<DbParameters>()), Times.Once);
			Assert.True(dbFields.Count == propertyCount - 1);
			Assert.True(dbParameters.Count == 1);
			Assert.Equal(expected, result);
		}

		[Fact]
		public void Update_TableNameModelMoqIgnoreFields_Success()
		{
			// Arranje
			var idField = "Id";
			var ignoreField = new[] { "version" };

			DbFields dbFields = null;
			DbParameters dbParameters = null;

			var tableName = this.fixture.Create<string>();
			var value = this.fixture.Create<ModelMoq>();
			var propertyCount = typeof(ModelMoq).GetProperties().Count();

			var expected = this.fixture.Create<int>();

			this.dbProviderMock
				.Setup(x => x.Update(It.IsAny<string>(), It.IsAny<DbFields>(), It.IsAny<string>(), It.IsAny<DbParameters>()))
				.Callback<string, DbFields, string, DbParameters>((param1, param2, param3, param4) =>
					{
						dbFields = param2;
						dbParameters = param4;
					})
				.Returns(expected);

			// Act
			var result = this.target.Update(tableName, value, idField, ignoreField);

			// Assert
			dbProviderMock.Verify(x => x.Update(tableName, dbFields, It.IsAny<string>(), It.IsAny<DbParameters>()), Times.Once);
			Assert.True(dbFields.Count == propertyCount - ignoreField.Count() - 1);
			Assert.True(dbParameters.Count == 1);
			Assert.Equal(expected, result);
		}

		#endregion Insert

		#region Transaction

		[Fact]
		public void WithTransactions_NoParams_Success()
		{
			// Arranje
			var transactionMock = new Mock<IDisposable>();

			this.dbProviderMock
				.Setup(x => x.WithTransactions())
				.Returns(transactionMock.Object);

			// Act
			var result = this.target.WithTransactions();

			// Assert
			dbProviderMock.Verify(x => x.WithTransactions(), Times.Once);
			Assert.NotNull(result);
		}

		[Fact]
		public void RollbackTransaction_NoParams_Success()
		{
			// Arranje


			// Act
			this.target.RollbackTransaction();

			// Assert
			dbProviderMock.Verify(x => x.RollbackTransaction(), Times.Once);
		}

		#endregion Transaction
	}
}
