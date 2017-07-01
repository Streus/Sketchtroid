using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;

#pragma warning disable 0168
[Serializable]
public class Extension : ISerializable
{
	/* Instance Vars */

	// The name of the prefab this Extension should create
	[SerializeField]
	private string prefabName;

	// The position a prefab will be placed when inst
	[SerializeField]
	private Vector3 prefabPos;

	// The rotation a prefab will be given on inst
	[SerializeField]
	private Vector3 prefabRot;

	// The GO this Extension handles
	public GameObject inst;

	// Components on inst that may be useful
	private Entity subEnt;
	private Destructable dest;
	private CollisionRelay relay;

	// The Entity that handles this Extension
	private Entity parent;

	/* Constructors */
	public Extension()
	{
		prefabName = "";
		inst = null;
		parent = null;
	}

	public Extension(SerializationInfo info, StreamingContext context)
	{
		prefabName = info.GetString ("prefabName");

		prefabPos = new Vector3 (
			info.GetSingle ("prefabPX"),
			info.GetSingle ("prefabPY"),
			info.GetSingle ("prefabPZ"));


		prefabRot = new Vector3 (
			info.GetSingle ("prefabRX"),
			info.GetSingle ("prefabRY"),
			info.GetSingle ("prefabRZ"));

		IReapable plot = inst.GetComponent<IReapable>();
		if (plot != null)
			plot.sow ((SeedBase)info.GetValue ("instSeed", typeof(SeedBase)));
	}

	/* Instance Methods */

	// Create a prefab if needed. Setup important connections
	public void init(Entity parent)
	{
		this.parent = parent;

		if (prefabName != "")
			inst = initGO (prefabName, prefabPos, Quaternion.Euler(prefabRot));

		//if the target instance has a CollisonRelay, add it to the parent's network
		relay = inst.GetComponent<CollisionRelay> ();
		if (relay != null)
			relay.addToNetwork (parent);

		//if the target is an Entity, listen to its destruction for detachment
		subEnt = inst.GetComponent<Entity>();
		if (subEnt != null)
			subEnt.died += detach;

		//if the target is destructable, listen to its destruction for detachment
		dest = inst.GetComponent<Destructable>();
		if (dest != null)
			dest.destructed += detach;
	}

	// Remove this Extension from its parent
	// Used for Extension-side cleanup of the Entity-Extension relationship
	public void detach()
	{
		//if there's nothing to detach, do nothing
		if (parent == null)
			return;

		parent.removeExtension (this);
	}

	// Cleanup inst depending on its components
	// Used for Entity-side cleanup of the Entity-Extension relationship
	public void cleanup()
	{
		if (subEnt != null)
		{
			//don't auto-remove, this is called in a foreach
			subEnt.died -= detach;

			//let Entity handle its own cleanup
			subEnt.OnDeath ();
		}
		else if (dest != null)
		{
			//don't auto-remove, this is called in a foreach
			dest.destructed -= detach;

			//let Destructable handle its own cleanup
			dest.OnDestroy();
		}
		if (relay != null)
		{
			relay.removeFromNetwork ();
			MonoBehaviour.Destroy (inst);
		}
	}

	private GameObject initGO(string name, Vector3 pos, Quaternion rot)
	{
		GameObject pref = Resources.Load<GameObject> ("Prefabs/" + prefabName);
		return (GameObject)MonoBehaviour.Instantiate (pref, pos, rot, parent.transform);
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue ("prefabName", prefabName);

		info.AddValue ("prefabPX", prefabPos.x);
		info.AddValue ("prefabPY", prefabPos.y);
		info.AddValue ("prefabPZ", prefabPos.z);

		info.AddValue ("prefabRX", prefabRot.x);
		info.AddValue ("prefabRY", prefabRot.y);
		info.AddValue ("prefabRZ", prefabRot.z);

		IReapable blade = inst.GetComponent<IReapable> ();
		if (blade != null)
			info.AddValue ("instSeed", blade.reap ());
	}
}
