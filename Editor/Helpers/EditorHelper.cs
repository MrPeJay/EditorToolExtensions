using System;
using EditorExtension.Attributes;
using UnityEditor;
using UnityEngine;

namespace EditorExtension.Helpers
{
    public static class EditorHelper
    {
        public static void DrawInColor(Color color, Action drawAction)
        {
            var prevColor = GUI.color;
            GUI.color = color;
            drawAction();
            GUI.color = prevColor;
        }

        /// <summary>
        /// Draws path selection field.
        /// </summary>
        /// <returns>Whether path value was changed</returns>
        public static bool DrawPathSelection(string currentSelectedPath, out string path, string fieldName, PathAttribute.PathType pathType = PathAttribute.PathType.File)
        {
            path = string.Empty;

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.TextField(fieldName, currentSelectedPath);
                PathAttributeDrawer.DrawPathSelectionLayout(new PathAttribute(pathType),
                    out var selectedPath, out var revert);

                if (!string.IsNullOrEmpty(selectedPath) || revert)
                {
                    path = selectedPath;
                    GUI.FocusControl(null);

                    return true;
                }
            }

            return false;
        }
    }
}
