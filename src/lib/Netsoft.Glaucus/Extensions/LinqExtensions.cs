// <copyright file="LinqExtensions.cs" company="Netsoft">
// Copyright (c) Netsoft. All rights reserved.
// </copyright>

namespace Netsoft.Glaucus
{
	using System.Collections.Generic;

	public static partial class LinqExtensions
	{
		public static string JoinToString(this IEnumerable<string> enumerable, string separator)
		{
			return string.Join(separator, enumerable);
		}
	}
}
