using System;
using UnityEngine;

namespace EditorExtension.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]

	public class PathAttribute : PropertyAttribute
	{
		public PathType Type;
		public string Extension;
		public string Directory;
		public string Name;

		public enum PathType
		{
			File,
			Folder
		}

		public PathAttribute(PathType type = PathType.File, string name = null, string extension = null, string directory = null)
		{
			Type = type;
			Extension = extension;
			Directory = string.IsNullOrEmpty(directory) ? Application.dataPath : directory;
			Name = string.IsNullOrEmpty(name) ? Type == PathType.File ? "Select File" : "Select Folder" : name;
		}
	}
}
