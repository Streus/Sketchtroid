using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;
using System.Reflection;
using UnityEngine.Scripting;

[Serializable]
public class Ability : ISerializable
{
	/* Static Vars */
	private static Dictionary<string, Ability> repository;

	static Ability()
	{
		repository = new Dictionary<string, Ability> ();
		repository.Add ("Basic Fire", new Ability ("Basic Fire", "Fire a bullet", "", 1f, "basicShoot"));
		repository.Add ("Spray", new Ability ("Spray", "Shoot a continuous stream of bullets", "", 0.1f, "sprayShoot"));
	}

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

	// Get an ability from the ability repository, ifex
	public static Ability get(string name)
	{
		Ability a;
		if (repository.TryGetValue (name, out a))
			return a;
		return null;
	}

	/* Constructors */
	public Ability(string name, string desc, string iconPath, float cooldownMax, string effect, string prereq = "", string preAnim = "", string postAnim = "")
	{
		this.name = name;
		this.desc = desc;
		this.iconPath = iconPath;
		if (iconPath != "")
			icon = Resources.Load<Sprite> (iconPath);
		else
			icon = null;

		this.cooldownMax = cooldownMax;
		_cooldownCurr = cooldownMax;

		effectName = effect;
		this.effect = (UseEffect)Delegate.CreateDelegate (
			typeof(UseEffect),
			this,
			typeof(Ability).GetMethod (effectName, BindingFlags.NonPublic | BindingFlags.Instance));

		checkName = prereq;
		if (checkName != "")
			check = (PrereqCheck)Delegate.CreateDelegate (
				typeof(PrereqCheck),
				this,
				typeof(Ability).GetMethod (checkName, BindingFlags.NonPublic | BindingFlags.Instance));
		else
			check = null;
		
		this.preAnim = preAnim;
		this.postAnim = postAnim;

		active = false;
		available = true;
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
		effect = (UseEffect)Delegate.CreateDelegate (
			typeof(UseEffect),
			this,
			typeof(Ability).GetMethod (effectName, BindingFlags.NonPublic | BindingFlags.Instance));

		checkName = info.GetString ("prereq");
		if (checkName != "")
			check = (PrereqCheck)Delegate.CreateDelegate (
				typeof(PrereqCheck),
				this,
				typeof(Ability).GetMethod (checkName, BindingFlags.NonPublic | BindingFlags.Instance));
		else
			check = null;
		
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
		if (!active)
			return;

		_cooldownCurr -= time;
		if (_cooldownCurr < 0f)
			_cooldownCurr = 0f;
	}

	// Called to use the Ability
	public bool use(Entity subject, Vector2 targetPosition, params object[] args)
	{
		if (!isReady ())
			return false;

		if (check != null && !check (subject))
			return false;

		if (effect (subject, targetPosition, args))
		{
			_cooldownCurr = cooldownMax;
			return true;
		}
		return false;
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
			"Cooldown: " + cooldownCurr.ToString ("##0.0") + " / " + cooldownMax.ToString ("##0.0") + "\n" +
			"Effect: " + effect.Method.ToString() + "\n" +
			"Prereq: " + checkName + "\n" +
			"PreAnim: " + preAnim + "\n" +
			"PostAnim: " + postAnim + "\n";
	}

	/* Use Effects */
	//apply each with [Preserve] to prevent linker from removing methods

	// Just a basic bullet-shooting ability
	private bool basicShoot(Entity subject, Vector2 targetPosition, params object[] args)
	{
		try
		{
			Bullet.create ("Basic", subject, (DamageType)args [0], subject.getFaction ());
			return true;
		}
		#pragma warning disable 0168
		catch(InvalidCastException ice)
		#pragma warning restore 0168
		{ 
			Debug.LogError ("Passed invalid argument to basicShoot");
		}
		return false;
	}

	// The Player's first basic ability
	private bool sprayShoot(Entity subject, Vector2 targetPosition, params object[] args)
	{
		try
		{
			Bullet b = Bullet.create ("Basic", subject, (DamageType)args [0], subject.getFaction ());
			b.transform.position += subject.transform.up + (Vector3)UnityEngine.Random.insideUnitCircle * 0.3f;
			return true;
		}
		#pragma warning disable 0168
		catch(InvalidCastException ice)
		#pragma warning restore 0168
		{ 
			Debug.LogError ("Passed invalid argument to sprayShoot");
		}
		return false;
	}

	/* Prereq Checks */
	//apply each with [Preserve] to prevent linker from removing methods

	/* Delegates and Events */

	// The effect that will occur when this ability is used
	private delegate bool UseEffect(Entity subject, Vector2 targetPosition, params object[] args);

	// A secondary boolean check to run before running an effect
	private delegate bool PrereqCheck(Entity Subject);
}
