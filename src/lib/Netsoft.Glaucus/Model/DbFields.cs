// <copyright file="DbFields.cs" company="Netsoft">
// Copyright (c) Netsoft. All rights reserved.
// </copyright>

namespace Netsoft.Glaucus
{
	using System.Collections.Generic;

	public class DbFields : Dictionary<string, object>
	{
		public DbFields()
		{
		}

		public DbFields(IEnumerable<KeyValuePair<string, object>> collection)
			: base(collection)
		{
		}

		public DbFields(IDictionary<string, object> dictionary)
			: base(dictionary)
		{
		}
	}
}
