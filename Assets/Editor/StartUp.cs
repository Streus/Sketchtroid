using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class StartUp
{
	static StartUp()
	{
		EditorApplication.playmodeStateChanged += initStart;
	}

	private static void initStart()
	{
		if (EditorApplication.isPlaying)
		{
			RegisteredObject.allowGeneration = false;
			EditorApplication.delayCall += initComplete;
			Debug.Log ("Preserving ROIDs");
		}
	}

	private static void initComplete()
	{
		RegisteredObject.allowGeneration = true;
		EditorApplication.delayCall -= initComplete;

		Debug.Log ("ROIDS can be modified");
	}
}
