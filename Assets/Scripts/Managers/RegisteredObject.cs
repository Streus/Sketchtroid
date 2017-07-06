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

	// For first-time spawning of prefabs that should be tracked by the SSM
	public static GameObject create(string prefabPath, Vector3 position, Quaternion rotation)
	{
		GameObject go = Resources.Load<GameObject> ("Prefabs/" + prefabPath);
		GameObject inst = Instantiate (go, position, rotation);
		RegisteredObject ro = inst.GetComponent<RegisteredObject> ();
		ro.Reset ();
		ro.prefabPath = prefabPath;
		return inst;
	}

	// For respawning a prefab in the sow cycle
	public static GameObject recreate(string prefabPath, string registeredID)
	{
		GameObject go = Resources.Load<GameObject> ("Prefabs/" + prefabPath);
		GameObject inst = Instantiate (go, Vector3.zero, Quaternion.identity);
		inst.GetComponent<RegisteredObject> ().registeredID = registeredID;
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
			throw new ReapException ("Registered Object " + ToString() + " has no values to reap");
		Debug.Log (ToString () + " reaped values."); //DEBUG

		//pass in a prefabPath so that if this RO is a prefab, it can be spawned again later
		SeedBase seed = blade.reap();
		seed.prefabPath = prefabPath;
		if (prefabPath != "")
			seed.registeredID = registeredID;

		return seed;
	}

	// Take a seed and pass it along to the reapable script attached to this GO
	public void sow(SeedBase seed)
	{
		IReapable hole = GetComponent<IReapable> ();
		if (hole == null)
			throw new ReapException ("Registered Object " + ToString() + " has nowhere to sow values");
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