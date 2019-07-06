namespace Netsoft.Glaucus.Tests.Moq
{
	using System;
	using System.Data;
	using System.Data.Common;
	using AutoFixture;
	using global::Moq;
	using Netsoft.Glaucus.Providers;

	public class DbProviderFake : DbProviderBase
	{
		private readonly Fixture fixture = new Fixture();
		private readonly Mock<IDbConnection> connectionMock;
		//private readonly Mock<DbDataAdapter> dataAdapterMock;

		public DbProviderFake()
		{
			//this.dataAdapterMock = this.CreateDbDataAdapterMock();

			this.connectionMock = this.CreateDbConnectionMock();
			base.Connection = this.connectionMock.Object;



		}



		protected override DbDataAdapter GetDataAdapter()
		{
			return new DbDataAdapterFake();
		//return this.dataAdapterMock.Object;
		}

		//private Mock<DbDataAdapter> CreateDbDataAdapterMock()
		//{
		//	var dataAdapterMock = new Mock<DbDataAdapter>();

		//	var dataTable = new DataTable();
		//	dataTable.Columns.Add("Id", typeof(Guid));
		//	dataTable.Columns.Add("StringValue", typeof(string));
		//	dataTable.Columns.Add("IntValue", typeof(int));
		//	dataTable.Columns.Add("DoubleValue", typeof(double));
		//	dataTable.Columns.Add("DateTimeValue", typeof(DateTime));

		//	for (int i = 0; i < 10; i++)
		//	{
		//		DataRow dataRow = dataTable.NewRow();

		//		dataRow["Id"] = Guid.NewGuid();
		//		dataRow["StringValue"] = this.fixture.Create<string>();
		//		dataRow["IntValue"] = this.fixture.Create<int>();
		//		dataRow["DoubleValue"] = this.fixture.Create<double>();
		//		dataRow["DateTimeValue"] = this.fixture.Create<DateTime>();

		//		dataTable.Rows.Add(dataRow);
		//	}

		//	dataAdapterMock
		//		.Setup(x => x.Fill(It.IsAny<DataTable>()))
		//		.Callback<DataTable>(x => x = dataTable);

		//	return dataAdapterMock;
		//}

		private Mock<IDbConnection> CreateDbConnectionMock()
		{
			var connectionMock = new Mock<IDbConnection>();
			var commandMock = new Mock<DbCommand>();

			connectionMock
				.Setup(x => x.CreateCommand())
				.Returns(commandMock.Object);

			return connectionMock;
		}

	}
}
