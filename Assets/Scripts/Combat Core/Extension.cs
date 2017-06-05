using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;

public class Extension : ISerializable
{
	/* Instance Vars */

	// The name of the prefab this Extension will inst when created
	private readonly string prefabName;

	// The GO this Extension handles
	private readonly GameObject inst;

	// The Entity that handles this Extension
	private readonly Entity parent;

	/* Constructor */
	public Extension(string prefabName, Entity parent)
	{
		this.prefabName = prefabName;
		this.parent = parent;

		//create GO instance
		GameObject pref = Resources.Load<GameObject> ("Prefabs/" + prefabName);
		inst = (GameObject)MonoBehaviour.Instantiate (pref, parent.transform, false);

		//sow values if they exist
		//TODO might need a different approch
	}

	public ISerializable getObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue ("prefabName", prefabName);
		IReapable blade = inst.GetComponent<IReapable> ();
		if (blade != null)
			info.AddValue ("instSeed", blade.reap ());
	}
}
