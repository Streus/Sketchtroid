using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;
using System.Reflection;

[Serializable]
public sealed partial class Ability : ISerializable
{
	#region STATIC_VARS

	private static Dictionary<string, Ability> repository;

	// The resources directory that holds all ability icons
	private const string ICON_DIR = "core";

	// The latest ID number not assigned to an ability
	private static int latestID;
	#endregion

	#region INSTANCE_VARS

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
	public float CooldownCurr { get { return _cooldownCurr; } }

	// The maximum possible cooldown value
	public readonly float CooldownMax;

	// The number of use charges this ability has accrued (<= chargesMax)
	private int _charges;
	public int Charges { get { return _charges; } }

	// The maximum number of use charges this ability can accrue
	public readonly int ChargesMax;

	// Delegate pointing the the method that will run when this ability is used
	private UseEffect effect;
	private string effectName;

	private PrereqCheck check;
	private string checkName;

	// Names of animations to run before and after the use effect
	public readonly string preAnim;
	public readonly string postAnim;

	// Can this Ability be activated?
	private int available;
	public bool Available
	{
		get { return available <= 0; }
		set { available += value ? 1 : -1; }
	}

	// Is this Ability's cooldown being updated?
	private int active;
	public bool Active
	{
		get { return active <= 0; }
		set { active += value ? 1 : -1; }
	}

	// Persistent data intended to carry over between invokes of this Ability
	private ISerializable persData;
	#endregion

	#region STATIC_METHODS

	// Get an ability from the ability repository, ifex
	public static Ability Get(string name)
	{
		Ability a;
		if (name != null && repository.TryGetValue (name, out a))
			return new Ability(a);
		return null;
	}

	// Add an ability to the ability repository
	private static void Put(Ability a)
	{
		repository.Add (a.name, a.AssignID());
	}

	#endregion

	#region INSTANCE_METHODS

	private Ability(string name, string desc, string iconPath, float cooldownMax, int chargesMax, string effect, string prereq = "", string preAnim = "", string postAnim = "")
	{
		this.id = -1;
		this.name = name;
		this.desc = desc;
		this.iconPath = iconPath;
		if (iconPath != "")
			icon = ABU.LoadAsset<Sprite> (ICON_DIR, iconPath);
		else
			icon = null;

		this.CooldownMax = cooldownMax;
		_cooldownCurr = cooldownMax;

		this.ChargesMax = chargesMax;
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

		Active = false;
		Available = true;

		persData = null;
	}
	public Ability(Ability a) : this (a.name, a.desc, a.iconPath, a.CooldownMax, a.ChargesMax, a.effectName, a.checkName, a.preAnim, a.postAnim){ this.id = a.id; }
	public Ability(SerializationInfo info, StreamingContext context)
	{
		id = info.GetInt32 ("id");
		name = info.GetString ("name");
		desc = info.GetString ("desc");
		iconPath = info.GetString ("iconPath");
		icon = ABU.LoadAsset<Sprite> (ICON_DIR, iconPath);

		CooldownMax = info.GetSingle ("cooldownMax");
		_cooldownCurr = info.GetSingle ("cooldownCurr");

		ChargesMax = info.GetInt32 ("chargesMax");
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
		Available = info.GetBoolean ("available");
		Active = info.GetBoolean ("active");

		persData = (ISerializable)info.GetValue ("persData", typeof(ISerializable));
	}

	/* Instance Methods */

	// Can this Ability be used?
	public bool IsReady()
	{
		return (_cooldownCurr <= 0 || Charges > 0) && Active && Available;
	}

	// Return the percentage of the cooldown that has been completed
	public float CooldownPercentage
	{
		get { return _cooldownCurr / CooldownMax; }
	}

	// Update the cooldown in accordance with the time the last update took
	public void UpdateCooldown(float time)
	{
		if (!Active)
			return;

		_cooldownCurr -= time;
		if (_cooldownCurr <= 0f)
		{
			_cooldownCurr = 0f;
			if(_charges < ChargesMax)
			{
				_charges++;
				if(_charges != ChargesMax)
					_cooldownCurr = CooldownMax;
			}
		}
	}

	public void InitPersData(ISerializable data)
	{
		persData = data;
	}

	// Called to use the Ability
	public bool Use(Entity subject, Vector2 targetPosition, params object[] args)
	{
		if (!IsReady ())
			return false;

		if (check != null && !check (subject))
			return false;

		if (effect (subject, targetPosition, args))
		{
			if (_charges > 0)
				_charges--;
			if (_charges < ChargesMax || ChargesMax == 0)
				_cooldownCurr = CooldownMax;
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

		info.AddValue ("cooldownCurr", CooldownCurr);
		info.AddValue ("cooldownMax", CooldownMax);

		info.AddValue ("charges", _charges);
		info.AddValue ("chargesMax", ChargesMax);

		info.AddValue ("effect", effectName);

		info.AddValue ("prereq", checkName);

		info.AddValue ("preAnim", preAnim);
		info.AddValue ("postAnim", postAnim);

		info.AddValue ("available", Available);
		info.AddValue ("active", Active);

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
			"Cooldown: " + CooldownCurr.ToString ("##0.0") + " / " + CooldownMax.ToString ("##0.0") + "\n" +
			"Charges: " + _charges + " / " + ChargesMax + "\n" + 
			"Effect: " + effect.Method.ToString() + "\n" +
			"Prereq: " + checkName + "\n" +
			"PreAnim: " + preAnim + "\n" +
			"PostAnim: " + postAnim + "\n";
	}

	// Setting of ID values
	private Ability AssignID()
	{
		id = Ability.latestID++;
		return this;
	}

	#endregion

	#region INTERNAL_TYPES

	// The effect that will occur when this ability is used
	private delegate bool UseEffect(Entity subject, Vector2 targetPosition, params object[] args);

	// A secondary boolean check to run before running an effect
	private delegate bool PrereqCheck(Entity Subject);
	#endregion
}
