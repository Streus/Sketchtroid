using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;

[DisallowMultipleComponent]
public class RegisteredObject : MonoBehaviour
{
	#region STATIC_VARS
	private static List<RegisteredObject> directory;
	static RegisteredObject()
	{ 
		directory = new List<RegisteredObject>();
	}
	#endregion

	#region INSTANCE_VARS
	[SerializeField]
	private string registeredID;
	public string rID
	{
		get { return registeredID; }
	}

	// Path to a prefab to which this RO is attached
	private string prefabPath = "";

	[SerializeField]
	private bool ignoreReset = false;
	#endregion

	#region STATIC_METHODS
	public static RegisteredObject[] getObjects()
	{
		return directory.ToArray ();
	}

	public static RegisteredObject findObject(string ID)
	{
		return directory.Find (delegate(RegisteredObject obj) {
			return obj.registeredID == ID;
		});
	}

	// For first-time spawning of prefabs that should be tracked by the SSM
	public static GameObject create(string prefabPath, Vector3 position, Quaternion rotation)
	{
		return create (prefabPath, position, rotation, null);
	}
	public static GameObject create(string prefabPath, Vector3 position, Quaternion rotation, Transform parent)
	{
		GameObject go = Resources.Load<GameObject> ("Prefabs/" + prefabPath);
		GameObject inst;
		if(parent == null)
			inst = Instantiate (go, position, rotation);
		else
			inst = Instantiate (go, position, rotation, parent);
		RegisteredObject ro = inst.GetComponent<RegisteredObject> ();
		ro.Reset ();
		ro.prefabPath = prefabPath;
		return inst;
	}

	// For respawning a prefab in the sow cycle
	public static GameObject recreate(string prefabPath, string registeredID, string parentID)
	{
		GameObject go = Resources.Load<GameObject> ("Prefabs/" + prefabPath);
		GameObject inst;
		if (parentID == "")
			inst = Instantiate (go, Vector3.zero, Quaternion.identity);
		else
		{
			RegisteredObject parent = RegisteredObject.findObject (parentID);
			if (parent == null)
				throw new ArgumentException ("[RO] " + parentID + " does not exist!");
			inst = Instantiate (go, Vector3.zero, Quaternion.identity, parent.transform);
		}
		RegisteredObject ro = inst.GetComponent<RegisteredObject> ();
		ro.registeredID = registeredID;
		ro.prefabPath = prefabPath;
		return inst;
	}

	// Special wrapper method for the generic Monobehaviour#Destroy
	// Manages saving the destruction state of a RO
	public static void destroy(GameObject go)
	{
		RegisteredObject ro = go.GetComponent<RegisteredObject> ();
		if (ro != null)
			ro.saveDestruction ();
		Destroy (go);
	}
	#endregion

	#region INSTANCE_METHODS
	public void Reset()
	{
		registeredID = Convert.ToBase64String (Guid.NewGuid ().ToByteArray ()).TrimEnd('=');
	}

	public void Awake()
	{
		directory.Add (this);
	}

	public void OnDestroy()
	{
		directory.Remove (this);
	}

	// Get the reapable scripts attached to this GO and return their seeds
	public SeedCollection reap()
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
		if (prefabPath != "")
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
	public void sow(SeedCollection collection)
	{
		IReapable[] holes = GetComponents<IReapable> ();
		if (holes.Length <= 0)
			Console.println (ToString() + " has nowhere to sow values", Console.Tag.info);
		else
			Console.println (ToString () + " sowed values.", Console.Tag.info);

		//intercept and save prefabPath
		prefabPath = collection.prefabPath;

		collection.sowSeeds (gameObject, holes);
	}

	// Tells the SSM that a RO has been destroyed through gameplay
	private void saveDestruction()
	{
		if (prefabPath != "")
			return;

		SeedCollection seed = reap ();
		seed.destroyed = true;

		SceneStateManager.instance ().store (rID, seed);
	}

	public override string ToString ()
	{
		return "[RO] " + gameObject.name + " (" + registeredID + ")";
	}
	#endregion
}