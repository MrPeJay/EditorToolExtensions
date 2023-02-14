using System;
using UnityEngine;

namespace EditorExtension.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class EditorFieldAttribute : PropertyAttribute
	{
		public string Name;

		public EditorFieldAttribute(string name)
		{
			Name = name;
		}
	}
}
