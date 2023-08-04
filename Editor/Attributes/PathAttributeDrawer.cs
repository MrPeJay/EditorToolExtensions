using System;
using EditorExtension.Helpers;
using UnityEditor;
using UnityEngine;

namespace EditorExtension.Attributes
{
    [CustomPropertyDrawer(typeof(PathAttribute))]
    public class PathAttributeDrawer : PropertyDrawer
    {
        //Some Unity versions do not support layout classes when drawing properties. Change it if you have any issues.
        //This only applies to PROPERTY DRAWING.
        private const bool UseLayoutClass = true;

        private static string PathSelectionButtonName = "...", RevertPathButtonName = "X";
        private const float PathSelectionButtonWidth = 40f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var path = attribute as PathAttribute;

            if (path == null)
                return;

            if (!UseLayoutClass)
            {
                EditorGUI.PropertyField(
                    new Rect(position.x, position.y,
                        position.width - (2 + PathSelectionButtonWidth + PathSelectionButtonWidth / 2),
                        position.height),
                    property, label: new GUIContent(property.displayName));

                if (property.propertyType == SerializedPropertyType.String)
                {
                    DrawPathSelection(position, path, out var selectedPath, out var revert);

                    if (!string.IsNullOrEmpty(selectedPath) || revert)
                        property.stringValue = selectedPath;
                }
            }
            else
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(property, label: new GUIContent(property.displayName));

                    if (property.propertyType == SerializedPropertyType.String)
                    {
                        DrawPathSelectionLayout(path, out var selectedPath, out var revert);

                        if (!string.IsNullOrEmpty(selectedPath))
                            property.stringValue = selectedPath;
                    }
                }
            }
        }

        private static void DrawPathSelection(Rect position, PathAttribute path, out string selectedPath,
            out bool revert)
        {
            selectedPath = string.Empty;
            revert = false;

            if (GUI.Button(
                    new Rect(position.width - PathSelectionButtonWidth - 2, position.y, PathSelectionButtonWidth,
                        position.height), PathSelectionButtonName))
            {
                switch (path.Type)
                {
                    case PathAttribute.PathType.File:
                        selectedPath = EditorUtility.OpenFilePanel(path.Name, path.Directory, path.Extension);
                        break;
                    case PathAttribute.PathType.Folder:
                        selectedPath = EditorUtility.OpenFolderPanel(path.Name, path.Directory, string.Empty);
                        break;
                }

                if (!string.IsNullOrEmpty(selectedPath))
                {
                    //Try to make it relative to project path.
                    selectedPath = AssetHelper.GetRelativePath(selectedPath);
                }

                return;
            }

            var prevColor = GUI.color;
            GUI.color = Color.red;

            if (GUI.Button(new Rect(position.width, position.y, PathSelectionButtonWidth / 2, position.height),
                    RevertPathButtonName))
            {
                selectedPath = string.Empty;
                revert = true;
            }

            GUI.color = prevColor;
        }

        /// <summary>
        /// Draws path selection field using layout classes.
        /// Warning: Can't use this method for property drawers as they no longer support Layout classes.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="selectedPath"></param>
        /// <param name="revert"></param>
        /// <param name="width"></param>
        public static void DrawPathSelectionLayout(PathAttribute path, out string selectedPath, out bool revert,
            int width = 40)
        {
            selectedPath = string.Empty;
            revert = false;

            if (GUILayout.Button(PathSelectionButtonName, GUILayout.MaxWidth(width)))
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
                    selectedPath = AssetHelper.GetRelativePath(selectedPath);
                }
            }

            var prevColor = GUI.color;
            GUI.color = Color.red;

            if (GUILayout.Button(RevertPathButtonName, GUILayout.MaxWidth(width / 2)))
            {
                selectedPath = string.Empty;
                revert = true;
            }

            GUI.color = prevColor;
        }
    }
}
