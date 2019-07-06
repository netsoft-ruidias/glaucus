// <copyright file="DeferredTransaction.cs" company="Netsoft">
// Copyright (c) Netsoft. All rights reserved.
// </copyright>

namespace Netsoft.Glaucus
{
	using System;
	using System.Data;
	using Netsoft.Glaucus.Providers;

	public class DeferredTransaction : IDisposable
	{
		private readonly DbProviderBase provider;

		internal DeferredTransaction(DbProviderBase provider)
		{
			this.provider = provider;

			if (this.provider.Connection.State == ConnectionState.Closed || this.provider.Connection.State == ConnectionState.Broken)
			{
				this.provider.Connection.Open();
			}

			if (!this.provider.IsTransaction)
			{
				this.provider.BeginTransaction();
			}
		}

		public void Dispose()
		{
			if (this.provider.IsTransaction)
			{
				this.provider.CommitTransaction();
			}
		}
	}
}
