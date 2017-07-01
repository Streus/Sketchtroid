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
	private uint registeredID;
	public uint rID
	{
		get { return registeredID; }
	}

	/* Static Methods */
	public static RegisteredObject[] getObjects()
	{
		return directory.ToArray ();
	}

	/* Instance Methods */
	public void Reset()
	{
		long root = ((uint)DateTime.Now.Ticks) + (DateTime.Now.Ticks >> 32);
		registeredID = (uint)root;
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
		return blade.reap ();
	}

	// Take a seed and pass it along to the reapable script attached to this GO
	public void sow(SeedBase seed)
	{
		IReapable hole = GetComponent<IReapable> ();
		if (hole == null)
			throw new ReapException ("Registered Object " + ToString() + " has nowhere to sow values");
		Debug.Log (ToString () + " sowed values."); //DEBUG
		hole.sow (seed);
	}

	public override string ToString ()
	{
		return rID.ToString ("0000000000");
	}
}

public class ReapException : ApplicationException
{
	public ReapException(string message) : base(message) { }
}