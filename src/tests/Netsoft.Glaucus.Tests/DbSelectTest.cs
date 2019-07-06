namespace Netsoft.Glaucus.Tests
{
	using AutoFixture;
	using Netsoft.Glaucus.Moq.Tests;
	using Netsoft.Glaucus.Tests.Moq;
	using Xunit;

	public class DbSelectTest
	{
		private readonly DbSelect target;
		private readonly Fixture fixture = new Fixture();

		public DbSelectTest()
		{
			this.target = this.CreateTarget();
		}

		[Fact]
		public void ToDataTable_NoParams_Success()
		{
			// Arranje

			// Act
			var result = this.target.ToDataTable();

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Rows.Count > 0);
		}

		[Fact]
		public void ToDataSet_TableName_Success()
		{
			// Arranje
			var tableName = this.fixture.Create<string>();

			// Act
			var result = this.target.ToDataSet(tableName);

			// Assert
			Assert.NotNull(result);
			Assert.True(result.Tables.Count > 0);
			Assert.True(result.Tables[0].Rows.Count > 0);
		}

		[Fact]
		public void ToList_NoParams_Success()
		{
			// Arranje

			// Act
			var result = this.target.ToList<ModelMoq>();

			// Assert
			Assert.NotNull(result);
		}



		private DbSelect CreateTarget()
		{
			var providerFake = new DbProviderFake();

			var query = this.fixture.Create<string>();
			var dbParameters = this.fixture.Create<DbParameters>();

			var target = providerFake.Select(
				query,
				dbParameters);

			return target;
		}
	}
}
