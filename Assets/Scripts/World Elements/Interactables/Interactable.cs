using UnityEngine;

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
	public abstract SeedBase reap();

	public abstract void sow(SeedBase s);

	public bool ignoreReset()
	{
		return !allowReset;
	}
}