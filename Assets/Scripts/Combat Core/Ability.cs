using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;
using System.Reflection;

[Serializable]
public class Ability : ISerializable
{
	/* Static Vars */


	/* Instance Vars */

	// The displayed name of this Ability
	public readonly string name;

	// The displayed description of this Ability
	public readonly string desc;

	// The path to the Sprite associated with this Ability
	private readonly string iconPath;

	// The Sprite associated with this Ability
	public readonly Sprite icon;

	// The current cooldown value
	private float _cooldownCurr;
	public float cooldownCurr{ get; }

	// The maximum possible cooldown value
	public readonly float cooldownMax;

	// Delegate pointing the the method that will run when this ability is used
	private UseEffect effect;
	private string effectName;

	private PrereqCheck check;
	private string checkName;

	// Names of animations to run before and after the use effect
	public readonly string preAnim;
	public readonly string postAnim;

	// Can this Ability be activated?
	public bool available;

	// Is this Ability's cooldown being updated?
	public bool active;

	/* Static Methods */


	/* Constructors */
	public Ability(string name, string desc, string iconPath, float cooldownMax, string effect, string prereq = "", string preAnim = "", string postAnim = "")
	{
		this.name = name;
		this.desc = desc;
		this.iconPath = iconPath;
		icon = Resources.Load<Sprite>(iconPath);

		this.cooldownMax = cooldownMax;
		_cooldownCurr = cooldownMax;

		effectName = effect;
		this.effect = (UseEffect)Delegate.CreateDelegate(typeof(Ability), typeof(Ability).GetMethod (effectName));

		checkName = prereq;
		if (checkName != "")
			check = (PrereqCheck)Delegate.CreateDelegate(typeof(Ability), typeof(Ability).GetMethod (checkName));
		
		this.preAnim = preAnim;
		this.postAnim = postAnim;
	}
	public Ability(Ability a) : this (a.name, a.desc, a.iconPath, a.cooldownMax, a.effectName, a.checkName, a.preAnim, a.postAnim){ }
	public Ability(SerializationInfo info, StreamingContext context)
	{
		name = info.GetString ("name");
		desc = info.GetString ("desc");
		iconPath = info.GetString ("iconPath");
		icon = Resources.Load<Sprite>(iconPath);

		cooldownMax = info.GetSingle ("cooldownMax");
		_cooldownCurr = info.GetSingle ("cooldownCurr");

		effectName = info.GetString ("effect");
		effect = (UseEffect)Delegate.CreateDelegate(typeof(Ability), typeof(Ability).GetMethod (effectName));

		checkName = info.GetString ("prereq");
		if (checkName != "")
			check = (PrereqCheck)Delegate.CreateDelegate(typeof(Ability), typeof(Ability).GetMethod (checkName));
		
		preAnim = info.GetString ("preAnim");
		postAnim = info.GetString ("postAnim");
		available = info.GetBoolean ("available");
		active = info.GetBoolean ("active");
	}

	/* Instance Methods */

	// Can this Ability be used?
	public bool isReady()
	{
		return _cooldownCurr <= 0 && active && available;
	}

	// Return the percentage of the cooldown that has been completed
	public float cooldownPercentage()
	{
		return _cooldownCurr / cooldownMax;
	}

	// Update the cooldown in accordance with the time the last update took
	public void updateCooldown(float time)
	{
		_cooldownCurr -= time;
		if (_cooldownCurr < 0f)
			_cooldownCurr = 0f;
	}

	// Called to use the Ability
	public bool use(Entity subject, Vector2 targetPosition)
	{
		if (!isReady ())
			return false;

		if (!check (subject))
			return false;

		return effect (subject, targetPosition);
	}

	// For serialization
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue ("name", name);
		info.AddValue ("desc", desc);
		info.AddValue ("iconPath", iconPath);

		info.AddValue ("cooldownCurr", cooldownCurr);
		info.AddValue ("cooldownMax", cooldownMax);

		info.AddValue ("effect", effectName);

		info.AddValue ("prereq", checkName);

		info.AddValue ("preAnim", preAnim);
		info.AddValue ("postAnim", postAnim);

		info.AddValue ("available", available);
		info.AddValue ("active", active);
	}

	// Equiv checks
	public override bool Equals (object obj)
	{
		return this.name.Equals (((Ability)obj).name);
	}
	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

	// String representation
	public override string ToString ()
	{
		return name + "\n" +
		desc + "\n" +
		"Icon: " + iconPath + "\n" +
		"Cooldown: " + cooldownCurr.ToString ("#00.0") + " / " + cooldownMax.ToString ("#00.0") + "\n" +
		"Effect: " + effectName + "\n" +
		"Prereq: " + checkName + "\n" +
		"PreAnim: " + preAnim + "\n" +
		"PostAnim: " + postAnim + "\n";
	}

	/* Use Effects */

	//TODO add ability effects here

	/* Prereq Checks */

	//TODO add custom prerequisite checks here

	/* Delegates and Events */

	// The effect that will occur when this ability is used
	private delegate bool UseEffect(Entity subject, Vector2 targetPosition);

	// A secondary boolean check to run before running an effect
	private delegate bool PrereqCheck(Entity Subject);
}
