﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;

[DisallowMultipleComponent]
[ExecuteInEditMode]
public class RegisteredObject : MonoBehaviour
{
	#region STATIC_VARS
	public static bool allowGeneration = false;

	private static List<RegisteredObject> directory;
	static RegisteredObject()
	{ 
		directory = new List<RegisteredObject>();
	}
	#endregion

	#region INSTANCE_VARS
	[SerializeField]
	private string registeredID;
	public string RID
	{
		get { return registeredID; }
	}

	// Used to detect duplicated objects
	[SerializeField]
	private int instanceID = 0;

	// Path to a prefab to which this RO is attached
	private string prefabPath = "";

	// Name of the prefab
	private string prefabName = "";

	// Does this RO's values persist through reset cycles?
	[SerializeField]
	private bool ignoreReset = false;

	// Is this RO tracked by the SSM?
	// Should only be enabled for the Player
	[SerializeField]
	private bool excludeFromDirectory = false;
	#endregion

	#region STATIC_METHODS
	public static RegisteredObject[] GetObjects()
	{
		return directory.ToArray ();
	}

	public static RegisteredObject FindObject(string ID)
	{
		return directory.Find (delegate(RegisteredObject obj) {
			return obj.registeredID == ID;
		});
	}

	// For first-time spawning of prefabs that should be tracked by the SSM
	public static GameObject Create(string prefabPath, string prefabName, Vector3 position, Quaternion rotation)
	{
		return Create (prefabPath, prefabName, position, rotation, null);
	}
	public static GameObject Create(string prefabPath, string prefabName, Vector3 position, Quaternion rotation, Transform parent)
	{
		if (prefabPath == "" || prefabName == "")
			throw new ArgumentException ("Cannot create prefab with empty path and/or name");

		GameObject go = ABU.LoadAsset<GameObject> (prefabPath, prefabName);
		GameObject inst;
		if(parent == null)
			inst = Instantiate (go, position, rotation);
		else
			inst = Instantiate (go, position, rotation, parent);
		RegisteredObject ro = inst.GetComponent<RegisteredObject> ();
		ro.GenerateID ();
		ro.prefabPath = prefabPath;
		ro.prefabName = prefabName;
		return inst;
	}

	// For respawning a prefab in the sow cycle
	public static GameObject Recreate(string prefabPath, string prefabName, string registeredID, string parentID)
	{
		if (prefabPath == "" || prefabName == "")
			throw new ArgumentException ("Cannot create prefab with empty path and/or name");

		GameObject go = ABU.LoadAsset<GameObject> (prefabPath, prefabName);
		GameObject inst;
		if (parentID == "")
			inst = Instantiate (go, Vector3.zero, Quaternion.identity);
		else
		{
			RegisteredObject parent = RegisteredObject.FindObject (parentID);
			if (parent == null)
				throw new ArgumentException ("[RO] " + parentID + " does not exist!");
			inst = Instantiate (go, Vector3.zero, Quaternion.identity, parent.transform);
		}
		RegisteredObject ro = inst.GetComponent<RegisteredObject> ();
		ro.registeredID = registeredID;
		ro.prefabPath = prefabPath;
		ro.prefabName = prefabName;
		return inst;
	}

	// Special wrapper method for the generic Monobehaviour#Destroy
	// Manages saving the destruction state of a RO
	public static void Destroy(GameObject go)
	{
		RegisteredObject ro = go.GetComponent<RegisteredObject> ();
		if (ro != null)
			ro.SaveDestruction ();
		UnityEngine.Object.Destroy (go);
	}
	#endregion

	#region INSTANCE_METHODS

	// Generate a new ID for this RO
	public bool GenerateID()
	{
#if UNITY_EDITOR
		if (!allowGeneration || UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
			return false;
#endif

		if (instanceID != GetInstanceID () || registeredID == "")
		{
			string oldROID = registeredID;
			instanceID = GetInstanceID ();
			registeredID = Convert.ToBase64String (Guid.NewGuid ().ToByteArray ()).TrimEnd ('=');

			Debug.LogWarning (ToString () + " generated a new ID.\n Old ID: " + oldROID);

			#if UNITY_EDITOR
			UnityEditor.EditorUtility.SetDirty(this);
			#endif

			return true;
		}
		return false;
	}

	public void Reset()
	{
		registeredID = "";
		instanceID = 0;
		GenerateID ();
	}

	public void Awake()
	{
		GenerateID();
#if UNITY_EDITOR
		if(UnityEditor.EditorApplication.isPlaying)
		{
#endif
			if(!excludeFromDirectory)
				directory.Add (this);
#if UNITY_EDITOR
		}
#endif

	}

	public void OnDestroy()
	{
#if UNITY_EDITOR
		if(UnityEditor.EditorApplication.isPlaying)
		{
#endif
			if(!excludeFromDirectory)
				directory.Remove (this);
#if UNITY_EDITOR
		}
#endif
	}

	// Get the reapable scripts attached to this GO and return their seeds
	public SeedCollection Reap()
	{
		IReapable[] blades = GetComponents<IReapable> ();
		if (blades.Length <= 0)
			Console.println (ToString() + " has no additional values to reap", Console.Tag.info);
		else
			Console.println (ToString () + " reaped values.", Console.Tag.info);

		SeedCollection collection = new SeedCollection (gameObject, blades);
		collection.ignoreReset = ignoreReset;

		//pass in a prefabPath so that if this RO is a prefab, it can be spawned again later
		collection.prefabPath = prefabPath;
		collection.prefabName = prefabName;
		if (prefabPath != "" && prefabName != "")
			collection.registeredID = registeredID;

		//pass in this RO's parent object, ifex
		Transform parent = gameObject.transform.parent;
		if (parent != null)
		{
			RegisteredObject parentRO = parent.GetComponent<RegisteredObject> ();
			if (parentRO != null)
				collection.parentID = parentRO.registeredID;
			else
				Console.println (ToString () + " is under a non-RO. Make its parent an RO to save its data properly.", Console.Tag.error);
		}
		else
			collection.parentID = "";

		return collection;
	}

	// Take a seed collection and distrubute it among the reapable scripts attached to this GO
	public void Sow(SeedCollection collection)
	{
		IReapable[] holes = GetComponents<IReapable> ();
		if (holes.Length <= 0)
			Console.println (ToString() + " has nowhere to sow values", Console.Tag.info);
		else
			Console.println (ToString () + " sowed values.", Console.Tag.info);

		//intercept and save prefabPath
		prefabPath = collection.prefabPath;
		prefabName = collection.prefabName;

		collection.SowSeeds (gameObject, holes);
	}

	// Tells the SSM that a RO has been destroyed through gameplay
	private void SaveDestruction()
	{
		if (prefabPath != "")
			return;

		SeedCollection seed = Reap ();
		seed.destroyed = true;

		SceneStateManager.GetInstance ().Store (RID, seed);
	}

	public bool GetIgnoreReset()
	{
		return ignoreReset;
	}

	public void SetIgnoreReset(bool val)
	{
		ignoreReset = val;
	}

	public bool GetExcludeFromDirectory()
	{
		return excludeFromDirectory;
	}

	public void SetExcludeFromDirectory(bool val)
	{
		//prevent modification from anywhere but in non-playing editor
		//changing this in runtime could cause some inconsistent behavior
#if UNITY_EDITOR
		if(UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
			return;
		excludeFromDirectory = val;
#endif
	}

	public override string ToString ()
	{
		return "[RO] " + gameObject.name + " (" + registeredID + ")";
	}
	#endregion
}