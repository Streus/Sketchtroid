using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Runtime.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class RegisteredObject : MonoBehaviour
{
	/* Static Vars */
	private static List<RegisteredObject> objects;

	/* Instance Vars */
	private uint registeredID;
	public uint rID
	{
		get { return registeredID; }
	}

	/* Instance Methods */
	public void Reset()
	{
		registeredID = (uint)DateTime.Now.Ticks;
	}

	public void Awake()
	{
		objects.Add (this);
	}

	public void OnDestroy()
	{
		objects.Remove (this);
	}

	public RegisteredObject[] getObjects()
	{
		return objects.ToArray ();
	}

	// Get the reapable script attached to this GO and return its seed
	public ISerializable reap()
	{
		IReapable blade = GetComponent<IReapable> ();
		if (blade == null)
			throw new ReapException ();
		return blade.reap ();
			
	}

	// Take a seed and pass it along to the reapable script attached to this GO
	public void sow(ISerializable seed)
	{
		IReapable hole = GetComponent<IReapable> ();
		if (hole == null)
			throw new ReapException ();
		hole.sow (seed);
	}
}

public class ReapException : ApplicationException { }