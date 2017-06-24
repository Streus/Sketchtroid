using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;

[Serializable]
public class Status : ISerializable
{
	/* Static Vars */


	/* Instance Vars */

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

	/* Static Methods */


	/* Constructors */
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

		this.components = new StatusComponent[components.Length];
		for(int i = 0; i < components.Length; i++)
		{
			this.components[i] = new StatusComponent(components[i]);
			this.components [i].stacks = stacks;
		}
	}
	public Status (Status s) : this (s.name, s.desc, s.iconPath, s.decayType, s.stacksMax, s.initDuration, s.components){ }
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
		{
			components[i] = (StatusComponent)info.GetValue ("component" + i, typeof(StatusComponent));
		}
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
			sc.OnRevert (subject);
			sc.stacks += dStacks;
			if(this.stacks > 0)
				sc.OnApply (subject);
		}
		this.stacks += dStacks;
	}

	// Called by the subject during the update loop
	public void updateDuration(Entity subject, float time)
	{
		OnUpdate (subject, time);

		duration -= time;
		if (duration <= 0f)
		{
			switch (decayType)
			{
			case DecayType.communal:
				OnStatusEnded ();
				break;
			case DecayType.serial:
				stack (subject, -1);
				if (stacks <= 0)
					OnStatusEnded ();
				break;
			}
		}
	}

	// --Hooks--

	// Called when this Status is first added to an Entity
	public void OnApply(Entity subject)
	{
		foreach (StatusComponent sc in components)
			sc.OnApply (subject);
	}

	// Called when this Status is removed from its subject
	public void OnRevert(Entity subject)
	{
		foreach (StatusComponent sc in components)
			sc.OnRevert (subject);
	}

	// Called every update cycle by the subject
	public void OnUpdate(Entity subject, float time)
	{
		foreach (StatusComponent sc in components)
			sc.OnUpdate (subject, time);
	}

	// Called whenever the subject takes damage
	public void OnDamageTaken(Entity subject, Entity attacker, float rawDamage, float calcDamage, DamageType dt, bool hitShields)
	{
		foreach (StatusComponent sc in components)
			sc.OnDamageTaken (subject, attacker, rawDamage, calcDamage, dt, hitShields);
	}

	// Called whenever the subject deals damage
	public void OnDamageDealt(Entity subject, Entity victim, float rawDamage, float calcDamage, DamageType dt, bool hitShields)
	{
		foreach (StatusComponent sc in components)
			sc.OnDamageDealt (subject, victim, rawDamage, calcDamage, dt, hitShields);
	}

	// Called when the subject dies
	public void OnDeath(Entity subject)
	{
		foreach (StatusComponent sc in components)
			sc.OnDeath (subject);
	}

	// Called when the subject's shields fall to zero
	public void OnShieldsDown(Entity subject)
	{
		foreach (StatusComponent sc in components)
			sc.OnShieldsDown (subject);
	}

	// Called when the subject's shields are fully recharged
	public void OnShieldsRecharged(Entity subject)
	{
		foreach (StatusComponent sc in components)
			sc.OnShieldsRecharged (subject);
	}

	// Called when the subject enters a stunned state
	public void OnStunned(Entity subject)
	{
		foreach (StatusComponent sc in components)
			sc.OnStunned (subject);
	}

	// Called when the subject enters a rooted state
	public void OnRooted(Entity subject)
	{
		foreach (StatusComponent sc in components)
			sc.OnRooted (subject);
	}

	// Called when the subject is healed
	public void OnHealed(Entity subject, float healAmount)
	{
		foreach (StatusComponent sc in components)
			sc.OnHealed (subject, healAmount);
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
	public void OnStatusEnded()
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

	/* Delegates and Events */
	public delegate void StatusEnded(Status s);
	public event StatusEnded durationCompleted;

	/* Inner classes, etc. */
	public enum DecayType
	{
		//stacks decay as a whole
		communal,

		//stacks decay one at a time
		serial
	}
}
