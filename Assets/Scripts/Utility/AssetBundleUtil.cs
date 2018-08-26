using System.IO;
using UnityEngine;

public static class ABU
{
	/// <summary>
	/// Because I'm lazy.
	/// </summary>
	/// <returns>The asset.</returns>
	/// <param name="bundlePath">Bundle path.</param>
	/// <param name="name">Name.</param>
	/// <typeparam name="T">Type of asset to load</typeparam>
	public static T LoadAsset<T>(string bundlePath, string name) where T : Object
	{
		if (bundlePath == "" || name == "")
			return default(T);

		//check loaded asset bundles first
		foreach (AssetBundle ab in AssetBundle.GetAllLoadedAssetBundles())
		{
			if (ab.Contains (name))
			{
				return ab.LoadAsset<T> (name);
			}
		}

		//load a new asset bundle
		AssetBundle bundle = AssetBundle.LoadFromFile (
			Application.streamingAssetsPath + 
			Path.DirectorySeparatorChar + 
			bundlePath);

		//load failed
		if (bundle == null)
		{
			return default(T);
		}

		Debug.Log ("Loaded new AssetBundle: " + bundle.name);

		//load succeeded, load object
		return bundle.LoadAsset<T> (name);
	}
}
