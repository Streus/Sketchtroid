using UnityEngine;
using System.Runtime.Serialization;

public class StatusComponent : ISerializable
{
	#region INSTANCE_VARS
	public int stacks;

	// Set by the parent Status when a component is added to it
	protected Status parent;
	#endregion

	#region INSTANCE_METHODS
	public StatusComponent()
	{
		stacks = 1;
		parent = null;
	}
	public StatusComponent(int stacks)
	{
		this.stacks = stacks;
		parent = null;
	}
	public StatusComponent(StatusComponent other) : this(other.stacks) { }
	public StatusComponent(SerializationInfo info, StreamingContext context)
	{
		stacks = info.GetInt32 ("stacks");
		parent = null;
	}

	// Sets the parent Status of this StatusComponent to the given Status
	public StatusComponent SetParent(Status s)
	{
		parent = s;
		return this;
	}

	// Called when this Status is first added to an Entity
	public virtual void OnApply(Entity subject){ }

	// Called when this Status is removed from its subject
	public virtual void OnRevert(Entity subject){ }

	// Called every update cycle by the subject
	public virtual void OnUpdate(Entity subject, float time){ }

	// Called whenever the subject takes damage
	public virtual void OnDamageTaken(Entity subject, Entity attacker, float rawDamage, float calcDamage, DamageType dt, bool hitShields){ }

	// Called whenever the subject deals damage
	public virtual void OnDamageDealt(Entity subject, Entity victim, float rawDamage, float calcDamage, DamageType dt, bool hitShields){ }

	// Called when the subject dies
	public virtual void OnDeath(Entity subject){ }

	// Called when the subject's shields fall to zero
	public virtual void OnShieldsDown(Entity subject){ }

	// Called when the subject's shields are fully recharged
	public virtual void OnShieldsRecharged(Entity subject){ }

	// Called when the subject enters a stunned state
	public virtual void OnStunned(Entity subject){ }

	// Called when the subject enters a rooted state
	public virtual void OnRooted(Entity subject){ }

	// Called when the subject is healed
	public virtual void OnHealed(Entity subject, float healAmount){ }

	// For serialization
	public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue ("stacks", stacks);
	}
	#endregion
}
