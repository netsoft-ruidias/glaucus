// <copyright file="TableAttribute.cs" company="Netsoft">
// Copyright (c) Netsoft. All rights reserved.
// </copyright>

namespace Netsoft.Glaucus
{
	using System;

	[AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
	public class TableAttribute : Attribute
	{
		public TableAttribute(string name)
		{
			this.Name = name;
		}

		public string Name { get; private set; } = string.Empty;
	}
}