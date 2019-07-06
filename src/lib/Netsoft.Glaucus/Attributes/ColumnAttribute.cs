// <copyright file="ColumnAttribute.cs" company="Netsoft">
// Copyright (c) Netsoft. All rights reserved.
// </copyright>

namespace Netsoft.Glaucus
{
	using System;

	[AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
	public class ColumnAttribute : Attribute
	{
		public ColumnAttribute(string name)
		{
			this.Name = name;
		}

		public string Name { get; private set; } = string.Empty;
	}
}
