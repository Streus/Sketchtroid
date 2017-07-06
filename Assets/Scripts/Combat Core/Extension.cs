using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;

#pragma warning disable 0168
[Serializable]
public class Extension : ISerializable, IEquatable<Extension>
{
	/* Instance Vars */

	// An ID derived from the root parent's ID
	[SerializeField]
	private string extensionID;
	public string eID { get { return extensionID; } }

	// The name of the prefab this Extension should create
	[SerializeField]
	private string prefabName;
	public string prefName { get { return prefabName; } }

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

	// The Seed taken from parent
	private SeedBase _seed;
	public SeedBase seed { get { return _seed; } set { _seed = value; } } 

	// Whether detach is in a event subcriber list
	private bool detachSubscribed;

	/* Constructors */
	public Extension()
	{
		prefabName = "";
		detachSubscribed = false;
	}
	public Extension(string prefabName, Vector3 position, Quaternion rotation, Entity parent)
	{
		this.prefabName = prefabName;
		this.parent = parent;
		initGO (prefabName, position, rotation);
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

		_seed = (SeedBase)info.GetValue ("instSeed", typeof(SeedBase));
		extensionID = _seed.registeredID;
	}

	/* Instance Methods */

	// Create a prefab if needed. Setup important connections
	public void init(Entity parent)
	{
		this.parent = parent;

		if (prefabName != "" && inst == null)
			inst = initGO (prefabName, prefabPos, Quaternion.Euler(prefabRot));

		//this is a dead extension, remove it
		if (inst == null)
		{
			detach ();
			return;
		}

		//if the target instance has a CollisonRelay, add it to the parent's network
		relay = inst.GetComponent<CollisionRelay> ();
		if (relay != null)
			relay.addToNetwork (parent);

		//if the target is an Entity, listen to its destruction for detachment
		subEnt = inst.GetComponent<Entity>();
		if (subEnt != null)
		{
			if (!detachSubscribed)
			{
				subEnt.died += detach;
				detachSubscribed = true;
			}
			if (seed != null)
				subEnt.sow (_seed);
		}

		//if the target is destructable, listen to its destruction for detachment
		dest = inst.GetComponent<Destructable>();
		if (dest != null)
		{
			if (!detachSubscribed)
			{
				dest.destructed += detach;
				detachSubscribed = true;
			}
			if (seed != null)
				dest.sow (_seed);
		}
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
			detachSubscribed = false;

			//let Entity handle its own cleanup
			subEnt.OnDeath ();
		}
		else if (dest != null)
		{
			//don't auto-remove, this is called in a foreach
			dest.destructed -= detach;
			detachSubscribed = false;

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

		info.AddValue ("prefabPX", inst.transform.position.x);
		info.AddValue ("prefabPY", inst.transform.position.y);
		info.AddValue ("prefabPZ", inst.transform.position.z);

		info.AddValue ("prefabRX", inst.transform.rotation.x);
		info.AddValue ("prefabRY", inst.transform.rotation.y);
		info.AddValue ("prefabRZ", inst.transform.rotation.z);

		IReapable blade = inst.GetComponent<IReapable> ();
		SeedBase seed = new SeedBase (inst);
		if (blade != null)
		{
			seed = blade.reap ();
			seed.registeredID = extensionID;
			info.AddValue ("instSeed", seed);
		}
	}

	public bool Equals (Extension other)
	{
		if (other == null)
			return false;
		return other.extensionID == extensionID;
	}
}
