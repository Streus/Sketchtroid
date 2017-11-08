using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;
using System.Reflection;

[Serializable]
public partial class Ability : ISerializable
{
	/* Static Vars */
	private static Dictionary<string, Ability> repository;

	// The resources directory that holds all ability icons
	private const string ICON_DIR = "Sprites/HUD/Abilities/";

	// The latest ID number not assigned to an ability
	private static int latestID;

	/* Instance Vars */

	// The unique ID for this ability type
	private int id;
	public int ID { get { return id; } }

	// The displayed name of this Ability
	[SerializeField]
	public readonly string name;

	// The displayed description of this Ability
	public readonly string desc;

	// The path to the Sprite associated with this Ability
	private readonly string iconPath;

	// The Sprite associated with this Ability
	public readonly Sprite icon;

	// The current cooldown value
	private float _cooldownCurr;
	public float cooldownCurr { get { return _cooldownCurr; } }

	// The maximum possible cooldown value
	public readonly float cooldownMax;

	// The number of use charges this ability has accrued (<= chargesMax)
	private int _charges;
	public int charges { get { return _charges; } }

	// The maximum number of use charges this ability can accrue
	public readonly int chargesMax;

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

	// Persistent data intended to carry over between invokes of this Ability
	private ISerializable persData;

	/* Static Methods */

	// Get an ability from the ability repository, ifex
	public static Ability get(string name)
	{
		Ability a;
		if (repository.TryGetValue (name, out a))
			return new Ability(a);
		return null;
	}

	/* Constructors */
	public Ability(string name, string desc, string iconPath, float cooldownMax, int chargesMax, string effect, string prereq = "", string preAnim = "", string postAnim = "")
	{
		this.id = -1;
		this.name = name;
		this.desc = desc;
		this.iconPath = iconPath;
		if (iconPath != "")
			icon = Resources.Load<Sprite> (ICON_DIR + iconPath);
		else
			icon = null;

		this.cooldownMax = cooldownMax;
		_cooldownCurr = cooldownMax;

		this.chargesMax = chargesMax;
		_charges = 0;

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

		persData = null;
	}
	public Ability(Ability a) : this (a.name, a.desc, a.iconPath, a.cooldownMax, a.chargesMax, a.effectName, a.checkName, a.preAnim, a.postAnim){ this.id = a.id; }
	public Ability(SerializationInfo info, StreamingContext context)
	{
		id = info.GetInt32 ("id");
		name = info.GetString ("name");
		desc = info.GetString ("desc");
		iconPath = info.GetString ("iconPath");
		icon = Resources.Load<Sprite>(ICON_DIR + iconPath);

		cooldownMax = info.GetSingle ("cooldownMax");
		_cooldownCurr = info.GetSingle ("cooldownCurr");

		chargesMax = info.GetInt32 ("chargesMax");
		_charges = info.GetInt32 ("charges");

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

		persData = (ISerializable)info.GetValue ("persData", typeof(ISerializable));
	}

	/* Instance Methods */

	// Can this Ability be used?
	public bool isReady()
	{
		return (_cooldownCurr <= 0 || charges > 0) && active && available;
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
		if (_cooldownCurr <= 0f)
		{
			_cooldownCurr = 0f;
			if(_charges < chargesMax)
			{
				_charges++;
				if(_charges != chargesMax)
					_cooldownCurr = cooldownMax;
			}
		}
	}

	public void initPersData(ISerializable data)
	{
		persData = data;
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
			if (_charges > 0)
				_charges--;
			if (_charges < chargesMax || chargesMax == 0)
				_cooldownCurr = cooldownMax;
			return true;
		}
		return false;
	}

	// For serialization
	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue ("id", id);
		info.AddValue ("name", name);
		info.AddValue ("desc", desc);
		info.AddValue ("iconPath", iconPath);

		info.AddValue ("cooldownCurr", cooldownCurr);
		info.AddValue ("cooldownMax", cooldownMax);

		info.AddValue ("charges", _charges);
		info.AddValue ("chargesMax", chargesMax);

		info.AddValue ("effect", effectName);

		info.AddValue ("prereq", checkName);

		info.AddValue ("preAnim", preAnim);
		info.AddValue ("postAnim", postAnim);

		info.AddValue ("available", available);
		info.AddValue ("active", active);

		info.AddValue ("persData", persData);
	}

	// Equiv checks
	public override bool Equals (object obj)
	{
		Ability other = (Ability)obj;
		if (other.id == -1 || this.id == -1)
			return this.name == other.name;
		return this.id == other.id;
	}
	public override int GetHashCode ()
	{
		return id;
	}

	// String representation
	public override string ToString ()
	{
		return name + "\n" +
			desc + "\n" +
			"Icon: " + ICON_DIR + iconPath + "\n" +
			"Cooldown: " + cooldownCurr.ToString ("##0.0") + " / " + cooldownMax.ToString ("##0.0") + "\n" +
			"Charges: " + _charges + " / " + chargesMax + "\n" + 
			"Effect: " + effect.Method.ToString() + "\n" +
			"Prereq: " + checkName + "\n" +
			"PreAnim: " + preAnim + "\n" +
			"PostAnim: " + postAnim + "\n";
	}

	// Setting of ID values
	private Ability assignID()
	{
		id = Ability.latestID++;
		return this;
	}

	/* Delegates and Events */

	// The effect that will occur when this ability is used
	private delegate bool UseEffect(Entity subject, Vector2 targetPosition, params object[] args);

	// A secondary boolean check to run before running an effect
	private delegate bool PrereqCheck(Entity Subject);
}
