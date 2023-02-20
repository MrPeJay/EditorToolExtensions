using System;
using UnityEditor;
using UnityEngine;

namespace EditorExtension.Extensions
{
	public class EditorWindowExtended : EditorWindow
	{
		protected static int CurrentPage, PageCount;

		protected void DrawPages()
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.LabelField($"{CurrentPage + 1}/{PageCount + 1}",
					new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter });
			}

			using (new EditorGUILayout.HorizontalScope())
			{
				if (CurrentPage == 0)
					GUI.enabled = false;

				if (GUILayout.Button("<<", GUILayout.MaxWidth(25)))
					CurrentPage = 0;

				if (GUILayout.Button("Previous Page"))
					CurrentPage--;

				GUI.enabled = true;

				if (CurrentPage == PageCount)
					GUI.enabled = false;

				if (GUILayout.Button("Next Page"))
					CurrentPage++;

				if (GUILayout.Button(">>", GUILayout.MaxWidth(25)))
					CurrentPage = PageCount;

				GUI.enabled = true;
			}
		}

		protected void DrawInColor(Color color, Action drawAction)
		{
			var prevColor = GUI.color;
			GUI.color = color;
			drawAction();
			GUI.color = prevColor;
		}
	}
}