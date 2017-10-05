using UnityEngine;
using System.Runtime.Serialization;

[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : MonoBehaviour, IReapable
{
	/* Instance Vars */
	public bool interactable;
	public bool activated{ get; protected set; }
	public bool bulletActivated;
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

	public virtual void OnInteract(DamageType dt)
	{
		if (!interactable || !bulletActivated)
			return;

		if (dt == keyType || keyType == DamageType.NONE)
			activated = !activated;
	}

	// --- IReapable methods ---
	public virtual SeedBase reap()
	{
		return new Seed (gameObject);
	}

	public virtual void sow(SeedBase s)
	{
		Seed seed = (Seed)s;

		interactable = seed.interactable;
		activated = seed.activated;
		bulletActivated = seed.bulletActivated;
		keyType = seed.keyType;
	}

	public bool ignoreReset()
	{
		return !allowReset;
	}

	[System.Serializable]
	protected class Seed : SeedBase
	{
		/* Instance Vars */
		public bool interactable, activated, bulletActivated;
		public DamageType keyType;

		/* Instance Methods */
		public Seed(GameObject subject) : base(subject)
		{
			Interactable i = subject.GetComponent<Interactable>();
			interactable = i.interactable;
			activated = i.activated;
			bulletActivated = i.bulletActivated;
			keyType = i.keyType;
		}
		public Seed(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			interactable = info.GetBoolean("interactable");
			activated = info.GetBoolean("activated");
			bulletActivated = info.GetBoolean("bActivated");
			keyType = (DamageType)info.GetInt32("keyType");
		}

		public override void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData (info, context);

			info.AddValue ("interactable", interactable);
			info.AddValue ("activated", activated);
			info.AddValue ("bActivated", bulletActivated);
			info.AddValue ("keyType", keyType);
		}
	}
}