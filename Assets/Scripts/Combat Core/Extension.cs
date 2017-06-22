using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;

public class Extension : ISerializable
{
	/* Instance Vars */

	// The name of the prefab this Extension should create
	private readonly string prefabName;

	// The GO this Extension handles
	public GameObject inst;

	// The Entity that handles this Extension
	private readonly Entity parent;

	/* Constructors */
	public Extension(Entity parent, string prefabName = "")
	{
		this.prefabName = prefabName;
		this.parent = parent;

		//create GO instance
		if (prefabName != "")
		{
			GameObject pref = Resources.Load<GameObject> ("Prefabs/" + prefabName);
			inst = (GameObject)MonoBehaviour.Instantiate (pref, parent.transform, false);
		}
	}

	public Extension(Entity parent, SerializationInfo info, StreamingContext context)
	{
		this.parent = parent;

		try { prefabName = info.GetString ("prefabName"); }
		catch(SerializationException se)
		{ prefabName = ""; }

		IReapable plot = inst.GetComponent<IReapable>();
		if (plot != null)
			plot.sow ((ISerializable)info.GetValue ("instSeed", typeof(ISerializable)));
	}

	private GameObject initGO(string name, Vector3 pos, Quaternion rot)
	{
		GameObject pref = Resources.Load<GameObject> ("Prefabs/" + prefabName);
		return (GameObject)MonoBehaviour.Instantiate (pref, pos, rot, parent.transform);
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue ("prefabName", prefabName);
		IReapable blade = inst.GetComponent<IReapable> ();
		if (blade != null)
			info.AddValue ("instSeed", blade.reap ());
	}
}
