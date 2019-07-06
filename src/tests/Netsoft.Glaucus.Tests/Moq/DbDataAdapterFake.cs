namespace Netsoft.Glaucus.Tests.Moq
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.Common;
	using System.Text;
	using AutoFixture;

	public class DbDataAdapterFake : DbDataAdapter
	{
		private readonly Fixture fixture = new Fixture();

		protected override int Fill(DataTable[] dataTables, int startRecord, int maxRecords, IDbCommand command, CommandBehavior behavior)
		{
			foreach (var dataTable in dataTables)
			{
				dataTable.Columns.Add("Id", typeof(Guid));
				dataTable.Columns.Add("StringValue", typeof(string));
				dataTable.Columns.Add("IntValue", typeof(int));
				dataTable.Columns.Add("DoubleValue", typeof(double));
				dataTable.Columns.Add("DateTimeValue", typeof(DateTime));

				for (int i = 0; i < 10; i++)
				{
					DataRow dataRow = dataTable.NewRow();

					dataRow["Id"] = Guid.NewGuid();
					dataRow["StringValue"] = this.fixture.Create<string>();
					dataRow["IntValue"] = this.fixture.Create<int>();
					dataRow["DoubleValue"] = this.fixture.Create<double>();
					dataRow["DateTimeValue"] = this.fixture.Create<DateTime>();

					dataTable.Rows.Add(dataRow);
				}
			}

			return dataTables[0].Rows.Count;		
		}

		protected override int Fill(DataSet dataSet, int startRecord, int maxRecords, string srcTable, IDbCommand command, CommandBehavior behavior)
		{
			var dataTable = new DataTable();
			dataTable.Columns.Add("Id", typeof(Guid));
			dataTable.Columns.Add("StringValue", typeof(string));
			dataTable.Columns.Add("IntValue", typeof(int));
			dataTable.Columns.Add("DoubleValue", typeof(double));
			dataTable.Columns.Add("DateTimeValue", typeof(DateTime));

			for (int i = 0; i < 10; i++)
			{
				DataRow dataRow = dataTable.NewRow();

				dataRow["Id"] = Guid.NewGuid();
				dataRow["StringValue"] = this.fixture.Create<string>();
				dataRow["IntValue"] = this.fixture.Create<int>();
				dataRow["DoubleValue"] = this.fixture.Create<double>();
				dataRow["DateTimeValue"] = this.fixture.Create<DateTime>();

				dataTable.Rows.Add(dataRow);
			}

			dataSet.Tables.Add(dataTable);

			return dataTable.Rows.Count;
		}
	}
}
