using UnityEngine;
using System.Runtime.Serialization;

[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : MonoBehaviour, IReapable
{
	/* Instance Vars */
	public bool interactable;
	public bool activated{ get; protected set; }
	public DamageType keyType;

	[SerializeField]
	private bool allowReset = true;

	/* Instance Methods */
	public virtual void OnInteract()
	{
		if (!interactable)
			return;

		activated = !activated;
	}

	public void OnInteract(DamageType dt)
	{
		if (!interactable)
			return;

		if (dt == keyType || keyType == DamageType.NONE)
		{
			activated = !activated;
			doInteract ();
		}
	}

	public abstract void doInteract ();

	// --- IReapable methods ---
	public virtual SeedCollection.Base reap()
	{
		return new Seed (gameObject);
	}

	public virtual void sow(SeedCollection.Base s)
	{
		Seed seed = (Seed)s;

		interactable = seed.interactable;
		activated = seed.activated;
		keyType = seed.keyType;
	}

	public bool ignoreReset()
	{
		return !allowReset;
	}

	[System.Serializable]
	protected class Seed : SeedCollection.Base
	{
		/* Instance Vars */
		public bool interactable, activated;
		public DamageType keyType;

		/* Instance Methods */
		public Seed(GameObject subject)
		{
			Interactable i = subject.GetComponent<Interactable>();
			interactable = i.interactable;
			activated = i.activated;
			keyType = i.keyType;
		}
		public Seed(SerializationInfo info, StreamingContext context)
		{
			interactable = info.GetBoolean("interactable");
			activated = info.GetBoolean("activated");
			keyType = (DamageType)info.GetInt32("keyType");
		}

		public override void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			info.AddValue ("interactable", interactable);
			info.AddValue ("activated", activated);
			info.AddValue ("keyType", keyType);
		}
	}
}