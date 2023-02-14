using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace EditorExtension.Helpers
{
	public static class AssetExtension
	{
		public class AssetInfo<T> where T : Object
		{
			public string PathToAsset;
			public T Asset;

			public AssetInfo(string pathToAsset, T asset)
			{
				PathToAsset = pathToAsset;
				Asset = asset;
			}
		}

		public static AssetInfo<T>[] GetAssetsOfTypeInPath<T>(string path, string extension) where T : Object
		{
			var typeFiles = Directory.GetFiles(path, extension, SearchOption.AllDirectories);

			var assetInfoList = new List<AssetInfo<T>>();

			for (var i = 0; i < typeFiles.Length; i++)
			{
				var pathToAsset = typeFiles[i];
				var loadedAsset = AssetDatabase.LoadAssetAtPath<T>(pathToAsset);

				assetInfoList.Add(new AssetInfo<T>(pathToAsset, loadedAsset));
			}

			return assetInfoList.ToArray();
		}

		public static string GetRelativePath(string absolutePath)
		{
			if (absolutePath.StartsWith(Application.dataPath))
				return "Assets" + absolutePath.Substring(Application.dataPath.Length);

			Debug.LogWarning("Specified path is not absolute");
			return absolutePath;
		}

		/// <summary>
		/// Filters can be found here: https://docs.unity3d.com/Manual/search-additional-searchfilters.html
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="filter"></param>
		/// <returns></returns>
		public static string GetPathOfAsset<T>(string filter)
		{
			var assets = AssetDatabase.FindAssets($"{filter} {nameof(T)}");
			return Path.GetDirectoryName(AssetDatabase.GUIDToAssetPath(assets[0]));
		}
	}
}