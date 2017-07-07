using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;

public class RegisteredObject : MonoBehaviour
{
	/* Static Vars */
	private static List<RegisteredObject> directory;
	static RegisteredObject()
	{ 
		directory = new List<RegisteredObject>();
	}

	/* Instance Vars */
	[SerializeField]
	private string registeredID;
	public string rID
	{
		get { return registeredID; }
	}

	// Path to a prefab to which this RO is attached
	private string prefabPath = "";

	/* Static Methods */
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

	/* Instance Methods */
	public void Reset()
	{
		registeredID = Convert.ToBase64String (Guid.NewGuid ().ToByteArray ());
	}

	public void Awake()
	{
		directory.Add (this);
	}

	public void OnDestroy()
	{
		directory.Remove (this);
	}

	// Get the reapable script attached to this GO and return its seed
	public SeedBase reap()
	{
		IReapable blade = GetComponent<IReapable> ();
		if (blade == null)
			throw new ReapException (ToString() + " has no values to reap");
		Debug.Log (ToString () + " reaped values."); //DEBUG

		SeedBase seed = blade.reap();

		//pass in a prefabPath so that if this RO is a prefab, it can be spawned again later
		seed.prefabPath = prefabPath;
		if (prefabPath != "")
			seed.registeredID = registeredID;

		//pass in this RO's parent object, ifex
		Transform parent = gameObject.transform.parent;
		if (parent != null)
		{
			RegisteredObject parentRO = parent.GetComponent<RegisteredObject> ();
			if (parentRO != null)
				seed.parentID = parentRO.registeredID;
			else
				Debug.LogError (ToString () + " is under a non-RO. Make its parent an RO to save its data properly."); //DEBUG
		}
		else
			seed.parentID = "";

		return seed;
	}

	// Take a seed and pass it along to the reapable script attached to this GO
	public void sow(SeedBase seed)
	{
		IReapable hole = GetComponent<IReapable> ();
		if (hole == null)
			throw new ReapException (ToString() + " has nowhere to sow values");
		Debug.Log (ToString () + " sowed values."); //DEBUG

		//intercept and save prefabPath
		prefabPath = seed.prefabPath;

		hole.sow (seed);
	}

	public override string ToString ()
	{
		return "[RO] ID: " + registeredID;
	}
}

public class ReapException : ApplicationException
{
	public ReapException(string message) : base(message) { }
}