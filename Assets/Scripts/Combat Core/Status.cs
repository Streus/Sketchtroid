using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;

[Serializable]
public sealed class Status : ISerializable
{
	#region STATIC_VARS

	#endregion

	#region INSTANCE_VARS

	// The displayed name of this Status
	public readonly string name;

	// The base description of this Status
	public readonly string desc;

	// The path to the Sprite associated with this Status
	public readonly string iconPath;

	// The Sprite associated with this Status
	public readonly Sprite icon;

	// Whether this status stacks effects with statuses of the same type
	public readonly DecayType decayType;

	// The number of stacks in this Status
	public int stacks { get; private set; }
	public readonly int stacksMax;

	// The time this Status will exist until it expires
	public float duration { get; private set; }

	// The initial duration value passed to this Status
	private readonly float initDuration;

	// The components that make up this Status
	private StatusComponent[] components;

	// Event that fires when this status's duration completes
	public event StatusEnded durationCompleted;
	#endregion

	#region STATIC_METHODS

	#endregion

	#region INSTANCE_METHODS

	public Status(string name, string desc, string iconPath, DecayType dt, int stacksMax, float duration, params StatusComponent[] components)
	{
		this.name = name;
		this.desc = desc;
		this.iconPath = iconPath;
		this.icon = Resources.Load<Sprite> (iconPath);

		decayType = dt;
		initDuration = duration;
		this.duration = initDuration;

		this.stacksMax = stacksMax;
		stacks = 1;

		this.components = components;
		for(int i = 0; i < components.Length; i++)
			this.components [i].setParent(this).stacks = stacks;
	}
	public Status (Status s) : this (s.name, s.desc, s.iconPath, s.decayType, s.stacksMax, s.initDuration, s.components) { }
	public Status(SerializationInfo info, StreamingContext context)
	{
		name = info.GetString ("name");
		desc = info.GetString ("desc");
		iconPath = info.GetString ("icon");
		icon = Resources.Load<Sprite> (iconPath);

		decayType = (DecayType)info.GetInt32 ("decayType");
		initDuration = info.GetSingle ("initDuration");
		duration = info.GetSingle ("duration");

		stacksMax = info.GetInt32 ("stacksMax");
		stacks = info.GetInt32 ("stacks");

		int numComponents = info.GetInt32 ("numComponents");
		this.components = new StatusComponent[numComponents];
		for (int i = 0; i < numComponents; i++)
			components[i] = ((StatusComponent)info.GetValue ("component" + i, typeof(StatusComponent))).setParent(this);
	}

	/* Instance Methods */

	// Stack this Status with another of the same type
	public void stack(Entity subject, int dStacks)
	{
		duration = initDuration;
		dStacks = Mathf.Clamp (dStacks, 0, stacksMax - stacks);
		if (dStacks == 0)
			return;
		foreach (StatusComponent sc in components)
		{
			sc.onRevert (subject);
			sc.stacks += dStacks;
			if(this.stacks > 0)
				sc.onApply (subject);
		}
		this.stacks += dStacks;
	}

	// Called by the subject during the update loop
	// Returns true if it ended.
	public bool updateDuration(Entity subject, float time)
	{
		onUpdate (subject, time);

		duration -= time;
		if (duration <= 0f)
		{
			switch (decayType)
			{
			case DecayType.communal:
				onStatusEnded ();
				return true;
			case DecayType.serial:
				stack (subject, -1);
				if (stacks <= 0)
				{
					onStatusEnded ();
					return true;
				}
				break;
			}
		}
		return false;
	}

	public float durationPercentage { get { return duration / initDuration; } }

	// Get a StatusComponent on this Status of type T
	public T getComponent<T>() where T : StatusComponent
	{
		foreach (StatusComponent sc in components)
			if (sc.GetType () == typeof(T))
				return (T)sc;
		return null;
	}

	// --Hooks--

	// Called when this Status is first added to an Entity
	public void onApply(Entity subject)
	{
		foreach (StatusComponent sc in components)
			sc.onApply (subject);
	}

	// Called when this Status is removed from its subject
	public void onRevert(Entity subject)
	{
		foreach (StatusComponent sc in components)
			sc.onRevert (subject);
	}

	// Called every update cycle by the subject
	public void onUpdate(Entity subject, float time)
	{
		foreach (StatusComponent sc in components)
			sc.onUpdate (subject, time);
	}

	// Called whenever the subject takes damage
	public void onDamageTaken(Entity subject, Entity attacker, float rawDamage, float calcDamage, DamageType dt, bool hitShields)
	{
		foreach (StatusComponent sc in components)
			sc.onDamageTaken (subject, attacker, rawDamage, calcDamage, dt, hitShields);
	}

	// Called whenever the subject deals damage
	public void onDamageDealt(Entity subject, Entity victim, float rawDamage, float calcDamage, DamageType dt, bool hitShields)
	{
		foreach (StatusComponent sc in components)
			sc.onDamageDealt (subject, victim, rawDamage, calcDamage, dt, hitShields);
	}

	// Called when the subject dies
	public void onDeath(Entity subject)
	{
		foreach (StatusComponent sc in components)
			sc.onDeath (subject);
	}

	// Called when the subject's shields fall to zero
	public void onShieldsDown(Entity subject)
	{
		foreach (StatusComponent sc in components)
			sc.onShieldsDown (subject);
	}

	// Called when the subject's shields are fully recharged
	public void onShieldsRecharged(Entity subject)
	{
		foreach (StatusComponent sc in components)
			sc.onShieldsRecharged (subject);
	}

	// Called when the subject enters a stunned state
	public void onStunned(Entity subject)
	{
		foreach (StatusComponent sc in components)
			sc.onStunned (subject);
	}

	// Called when the subject enters a rooted state
	public void onRooted(Entity subject)
	{
		foreach (StatusComponent sc in components)
			sc.onRooted (subject);
	}

	// Called when the subject is healed
	public void onHealed(Entity subject, float healAmount)
	{
		foreach (StatusComponent sc in components)
			sc.onHealed (subject, healAmount);
	}

	// For serialization
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue ("name", name);
		info.AddValue ("desc", desc);
		info.AddValue ("icon", iconPath);

		info.AddValue ("decayType", (int)decayType);
		info.AddValue ("initDuration", initDuration);
		info.AddValue ("duration", duration);

		info.AddValue ("stacksMax", stacksMax);
		info.AddValue ("stacks", stacks);

		info.AddValue ("numComponents", components.Length);
		for (int i = 0; i < components.Length; i++)
		{
			info.AddValue ("components" + i, components [i]);
		}
	}

	// Called when this Status's duration falls below zero
	public void onStatusEnded()
	{
		if (durationCompleted != null)
			durationCompleted (this);
	}

	// Equiv check
	public override bool Equals (object obj)
	{
		return this.name.Equals (((Status)obj).name);
	}
	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

	// String representation
	public override string ToString ()
	{
		return name + "\n" + desc + "\n" + duration.ToString("#00.0") + " / " + initDuration.ToString("#00.0");
	}

	#endregion

	#region INTERNAL_TYPES
	/* Delegates and Events */
	public delegate void StatusEnded(Status s);

	/* Inner classes, etc. */
	public enum DecayType
	{
		//stacks decay as a whole
		communal,

		//stacks decay one at a time
		serial
	}
	#endregion
}
