using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class Interactable : MonoBehaviour
{
	/* Instance Vars */
	public bool interactable;
	public bool activated{ get; protected set; }
	public bool bulletActivated;
	public DamageType keyType;

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
}