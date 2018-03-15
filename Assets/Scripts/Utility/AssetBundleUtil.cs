using System.IO;
using UnityEngine;

public static class AssetBundleUtil
{
	/// <summary>
	/// Because I'm lazy.
	/// </summary>
	/// <returns>The asset.</returns>
	/// <param name="bundlePath">Bundle path.</param>
	/// <param name="name">Name.</param>
	/// <typeparam name="T">Type of asset to load</typeparam>
	public static T loadAsset<T>(string bundlePath, string name) where T : UnityEngine.Object
	{
		if (bundlePath == "" || name == "")
			return default(T);

		//check loaded asset bundles first
		foreach (AssetBundle ab in AssetBundle.GetAllLoadedAssetBundles())
		{
			if (ab.Contains (name))
			{
				Debug.Log (ab.name + " is already loaded."); //DEBUG
				return ab.LoadAsset<T> (name);
			}
			else
			{
				Debug.Log (ab.name + " was unloaded."); //DEBUG
				ab.Unload (false);
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
			Debug.LogError ("AssetBundle " + bundlePath + " could not be loaded!");
			return default(T);
		}

		Debug.Log ("Loaded new AssetBundle: " + bundle.name);

		//load succeeded, load object
		return bundle.LoadAsset<T> (name);
	}
}
