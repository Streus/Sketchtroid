using System;
using UnityEngine;

public static class AssetBundleUtil
{
	/// <summary>
	/// Because I'm lazy.
	/// </summary>
	/// <returns>The asset.</returns>
	/// <param name="bundlePath">Bundle path.</param>
	/// <param name="name">Name.</param>
	/// <typeparam name="T">The type of the asset</typeparam>
	public static T loadAsset<T>(string bundlePath, string name)
	{
		if (bundlePath == "" || name == "")
			return null;

		AssetBundle bundle = AssetBundle.LoadFromFile (Application.streamingAssetsPath + bundlePath);
		if (bundle == null)
		{
			Debug.LogError ("Bundle " + bundlePath + " could not be loaded!");
			return default(T);
		}
		return bundle.LoadAsset<T> (name);
	}
}
