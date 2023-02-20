using System;
using EditorExtension.Helpers;
using UnityEditor;
using UnityEngine;

namespace EditorExtension.Attributes
{
	[CustomPropertyDrawer(typeof(PathAttribute))]
	public class PathAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var path = attribute as PathAttribute;

			if (path == null)
				return;

			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.PropertyField(property, label: new GUIContent(property.displayName));

				if (property.propertyType == SerializedPropertyType.String)
				{
					DrawPathSelection(path, out var selectedPath);

					if (!string.IsNullOrEmpty(selectedPath))
						property.stringValue = selectedPath;
				}
			}
		}

		///Hacky way of fixing empty spaces when drawing this property.
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) { return 0; }

		public static void DrawPathSelection(PathAttribute path, out string selectedPath, int width = 40)
		{
			selectedPath = string.Empty;
			
			if (GUILayout.Button("...", GUILayout.MaxWidth(width)))
			{
				switch (path.Type)
				{
					case PathAttribute.PathType.File:
						selectedPath = EditorUtility.OpenFilePanel(path.Name, path.Directory, path.Extension);
						break;
					case PathAttribute.PathType.Folder:
						selectedPath = EditorUtility.OpenFolderPanel(path.Name, path.Directory, string.Empty);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}

				if (!string.IsNullOrEmpty(selectedPath))
				{
					//Try to make it relative to project path.
					selectedPath = AssetExtension.GetRelativePath(selectedPath);
				}
			}
		}
	}
}
