// <copyright file="DbParameters.cs" company="Netsoft">
// Copyright (c) Netsoft. All rights reserved.
// </copyright>

namespace Netsoft.Glaucus
{
	using System.Collections.Generic;

	public class DbParameters : Dictionary<string, object>
	{
		public DbParameters()
		{
		}

		public DbParameters(IEnumerable<KeyValuePair<string, object>> collection)
			: base(collection)
		{
		}

		public DbParameters(IDictionary<string, object> dictionary)
			: base(dictionary)
		{
		}
	}
}
