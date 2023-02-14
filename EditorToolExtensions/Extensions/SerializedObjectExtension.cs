using System;
using System.Linq;
using System.Reflection;
using EditorExtension.Attributes;
using UnityEditor;
using UnityEngine;

namespace EditorExtension.Extensions
{
	public static class SerializedObjectExtension
	{
		/// <summary>
		/// Gets field name of field that uses EditorField attribute and matches the specified editorFieldValue parameter value.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="editorFieldValue"></param>
		/// <returns></returns>
		private static string GetFieldName(Type type, string editorFieldValue)
		{
			var fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
				.Where(prop => prop.IsDefined(typeof(EditorFieldAttribute), false)).ToArray();

			for (var i = 0; i < fields.Length; i++)
			{
				var currentField = fields[i];

				var editorFieldAttribute = (EditorFieldAttribute)
					currentField.GetCustomAttributes(typeof(EditorFieldAttribute), false)[0];

				if (editorFieldAttribute.Name.Equals(editorFieldValue))
				{
					return currentField.Name;
				}
			}

			return null;
		}

		/// <summary>
		/// Searches and tries to find fields which use EditorField attribute and
		/// matches the specified editorPropertyFieldName value.
		/// </summary>
		/// <param name="serializedObject"></param>
		/// <param name="editorFieldValue"></param>
		/// <returns>Field SerializedProperty which matches the specified name</returns>
		public static SerializedProperty FindPropertyByEditorField(this SerializedObject serializedObject,
			string editorFieldValue)
		{
			var targetType = serializedObject.targetObject.GetType();

			var fieldName = GetFieldName(targetType, editorFieldValue);

			if (!string.IsNullOrEmpty(fieldName))
				return serializedObject.FindProperty(fieldName);

			Debug.LogError(
				$"Couldn't find serialized property in type {targetType.Name} with specified name: {editorFieldValue}." +
				$" Please check if field uses the attribute and naming is set correctly.");

			return null;
		}

		/// <summary>
		/// Searches and tries to find fields which use EditorField attribute and
		/// matches the specified editorPropertyFieldName value.
		/// </summary>
		/// <param name="serializedProperty"></param>
		/// <param name="editorFieldValue"></param>
		/// <returns>Relative field SerializedProperty which matches the specified name</returns>
		public static SerializedProperty FindPropertyRelativeByEditorField(this SerializedProperty serializedProperty,
			string editorFieldValue)
		{
			var propertyObject = GetTargetObjectOfProperty(serializedProperty);

			if (propertyObject == null)
			{
				Debug.LogError(
					$"Couldn't find the type of serializedProperty with type name: {serializedProperty.managedReferenceFullTypename}");
				return null;
			}

			var type = propertyObject.GetType();

			var fieldName = GetFieldName(type, editorFieldValue);

			if (!string.IsNullOrEmpty(fieldName))
				return serializedProperty.FindPropertyRelative(fieldName);

			Debug.LogError(
				$"Couldn't find relative serialized property in type {type.Name} with specified name: {editorFieldValue}." +
				$" Please check if field uses the attribute and naming is set correctly.");

			return null;
		}

		/// <summary>
		/// Gets the object the property represents.
		/// </summary>
		/// <param name="prop"></param>
		/// <returns></returns>
		public static object GetTargetObjectOfProperty(SerializedProperty prop)
		{
			if (prop == null)
			{
				return null;
			}

			var path = prop.propertyPath.Replace(".Array.data[", "[");
			object obj = prop.serializedObject.targetObject;
			var elements = path.Split('.');

			for (var i = 0; i < elements.Length; i++)
			{
				var currentElement = elements[i];

				if (currentElement.Contains("["))
				{
					var elementName = currentElement.Substring(0, currentElement.IndexOf("["));
					var index = Convert.ToInt32(currentElement.Substring(currentElement.IndexOf("[")).Replace("[", "")
						.Replace("]", ""));
					obj = GetValue_Imp(obj, elementName, index);
				}
				else
				{
					obj = GetValue_Imp(obj, currentElement);
				}
			}

			return obj;
		}

		private static object GetValue_Imp(object source, string name)
		{
			if (source == null)
			{
				return null;
			}

			var type = source.GetType();

			while (type != null)
			{
				var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

				if (f != null)
				{
					return f.GetValue(source);
				}

				var p = type.GetProperty(name,
					BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

				if (p != null)
				{
					return p.GetValue(source, null);
				}

				type = type.BaseType;
			}

			return null;
		}

		private static object GetValue_Imp(object source, string name, int index)
		{
			var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;

			if (enumerable == null)
			{
				return null;
			}

			var enm = enumerable.GetEnumerator();

			for (var i = 0; i <= index; i++)
			{
				if (!enm.MoveNext())
				{
					return null;
				}
			}

			return enm.Current;
		}
	}
}
