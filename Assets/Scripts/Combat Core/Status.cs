using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;

[Serializable]
public sealed partial class Status : ISerializable
{
	#region STATIC_VARS

	private const string ICON_DIR = "core";

	// A repo for commonly used statuses
	private static Dictionary<string, Status> repo;

	// the latest ID not assigned to a status
	private static int latestID = 0;
	#endregion

	#region INSTANCE_VARS

	// The unique ID for this status
	private int _id;
	public int ID { get { return _id; } }

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
	public int Stacks { get; private set; }
	public readonly int StacksMax;

	// The time this Status will exist until it expires
	public float Duration { get; private set; }

	// The initial duration value passed to this Status
	private readonly float initDuration;

	// The components that make up this Status
	private StatusComponent[] components;
	#endregion

	#region STATIC_METHODS

	// Get a copy of the given status from the repository
	public static Status Get(string name, float duration = float.NaN)
	{
		Status s;
		if (repo.TryGetValue (name, out s))
		{
			if (float.IsNaN (duration))
				return new Status (s);
			return new Status (s, duration);
		}
		return null;
	}

	private static Status AssignID(Status s)
	{
		s._id = latestID++;
		return s;
	}

	private static void Put(Status s)
	{
		repo.Add (s.name, AssignID (s));
	}
	#endregion

	#region INSTANCE_METHODS

	public Status(string name, string desc, string iconPath, DecayType dt, int stacksMax, float duration, params StatusComponent[] components)
	{
		this.name = name;
		this.desc = desc;
		this.iconPath = iconPath;
		this.icon = ABU.LoadAsset<Sprite> (ICON_DIR, iconPath);

		decayType = dt;
		initDuration = duration;
		this.Duration = initDuration;

		this.StacksMax = stacksMax;
		Stacks = 1;

		this.components = components;
		for(int i = 0; i < components.Length; i++)
			this.components [i].SetParent(this).stacks = Stacks;
	}
	public Status (Status s) : this (s.name, s.desc, s.iconPath, s.decayType, s.StacksMax, s.initDuration, s.components) { }
	public Status(Status s, float duration) : this(s)
	{
		this.initDuration = duration;
		this.Duration = initDuration;
	}
	public Status(SerializationInfo info, StreamingContext context)
	{
		name = info.GetString ("name");
		desc = info.GetString ("desc");
		iconPath = info.GetString ("icon");
		icon = ABU.LoadAsset<Sprite> (ICON_DIR, iconPath);

		decayType = (DecayType)info.GetInt32 ("decayType");
		initDuration = info.GetSingle ("initDuration");
		Duration = info.GetSingle ("duration");

		StacksMax = info.GetInt32 ("stacksMax");
		Stacks = info.GetInt32 ("stacks");

		int numComponents = info.GetInt32 ("numComponents");
		this.components = new StatusComponent[numComponents];
		for (int i = 0; i < numComponents; i++)
			components[i] = ((StatusComponent)info.GetValue ("component" + i, typeof(StatusComponent))).SetParent(this);
	}

	/* Instance Methods */

	// Stack this Status with another of the same type
	public void Stack(Entity subject, int dStacks)
	{
		Duration = initDuration;
		dStacks = Mathf.Clamp (dStacks, 0, StacksMax - Stacks);
		if (dStacks == 0)
			return;
		foreach (StatusComponent sc in components)
		{
			sc.OnRevert (subject);
			sc.stacks += dStacks;
			if(this.Stacks > 0)
				sc.OnApply (subject);
		}
		this.Stacks += dStacks;
	}

	// Called by the subject during the update loop
	// Returns true if it ended.
	public bool UpdateDuration(Entity subject, float time)
	{
		OnUpdate (subject, time);

		Duration -= time;
		if (Duration <= 0f)
		{
			switch (decayType)
			{
			case DecayType.communal:
				return true;
			case DecayType.serial:
				Stack (subject, -1);
				if (Stacks <= 0)
					return true;
				break;
			}
		}
		return false;
	}

	public float DurationPercentage { get { return Duration / initDuration; } }

	// Get a StatusComponent on this Status of type T
	public T GetComponent<T>() where T : StatusComponent
	{
		foreach (StatusComponent sc in components)
			if (sc.GetType () == typeof(T))
				return (T)sc;
		return null;
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
	public void onStunned(Entity subject)
	{
		foreach (StatusComponent sc in components)
			sc.OnStunned (subject);
	}

	// Called when the subject enters a rooted state
	public void onRooted(Entity subject)
	{
		foreach (StatusComponent sc in components)
			sc.OnRooted (subject);
	}

	// Called when the subject is healed
	public void onHealed(Entity subject, float healAmount)
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
		info.AddValue ("duration", Duration);

		info.AddValue ("stacksMax", StacksMax);
		info.AddValue ("stacks", Stacks);

		info.AddValue ("numComponents", components.Length);
		for (int i = 0; i < components.Length; i++)
		{
			info.AddValue ("components" + i, components [i]);
		}
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
		return name + "\n" + desc + "\n" + Duration.ToString("#00.0") + " / " + initDuration.ToString("#00.0");
	}

	#endregion

	#region INTERNAL_TYPES

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
