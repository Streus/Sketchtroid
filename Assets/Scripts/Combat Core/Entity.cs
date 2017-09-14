﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;

public sealed class Entity : MonoBehaviour, IReapable
{
	/* Static Vars */
	private const float COMBAT_TIMER_MAX = 5f; // 5 seconds

	/* Instance Vars */

	// The faction this Entity belongs to. Entities of the same faction cannot hurt eachother
	[SerializeField]
	private Faction faction;

	[SerializeField]
	private bool allowReset = true;

	// A resource pool that is deducted from when taking damage. Death occurs when it reaches 0.
	private float health;
	public float healthMax = 75f;

	// A resource pool that is deducted before health. Can regen after a delay.
	private float shields;
	public float shieldsMax = 25f;
	public float shieldRegen = 1f;
	private float shieldDelay;
	public float shieldDelayMax = 2f;

	// Each reduces the damage taken from their respective damage types
	public Stat physResist = new Stat(0, 0, 100);
	public Stat elecResist = new Stat(0, 0, 100);
	public Stat biolResist = new Stat(0, 0, 100);
	public Stat cryoResist = new Stat(0, 0, 100);
	public Stat pyroResist = new Stat(0, 0, 100);
	public Stat voidResist = new Stat(0, 0, 100);

	// The speed at which this Entity can move through the world
	public Stat movespeed = new Stat(0, 0, 25);

	// If > 0, this Entity is in combat
	private float combatTimer;

	// If > 0, this Entity will not take damage
	private Stat invincible;

	// If > 0, this Entity's Controller cannot take any action
	private Stat stunned;

	// If > 0, this Entity cannot move
	private Stat rooted;

	// Used with Cryo damage type
	private float freezeProgress;

	// The Statuses currently affecting this Entity
	private List<Status> statuses;

	// The Abilities that this Entity
	[SerializeField]
	private List<Ability> abilities;

	// The Bullets with which this Entity and its netowrk have collided
	private HashSet<Bullet> collisonLog;

	/* Static Methods */
	public static void damageEntity(Entity victim, Entity attacker, float damage, DamageType dt, bool ignoreShields = false, params Status[] s)
	{
		//everyone is in combat
		victim.combatTimer = COMBAT_TIMER_MAX;
		if (attacker != null)
			attacker.combatTimer = COMBAT_TIMER_MAX;

		//don't deal negative damage; that's healing
		if (damage < 0)
			damage = 0;

		//victim is invincible, do nothing
		if (victim.isInvincible())
			return;

		//the actual damage value that will be subtracted from health/shields
		float calcDamage = damage;

		//do special dt-based effects and apply resistances
		switch (dt)
		{
		case DamageType.PHYSICAL:
			calcDamage *= (100f - (float)victim.physResist.value) / 100f;
			break;
		case DamageType.ELECTRIC:
			calcDamage *= (100f - (float)victim.elecResist.value) / 100f;
			//TODO special electric dt effect
			break;
		case DamageType.BIO:
			calcDamage *= (100f - (float)victim.biolResist.value) / 100f;
			//TODO special biological dt effect
			break;
		case DamageType.CRYO:
			calcDamage *= (100f - (float)victim.cryoResist.value) / 100f;
			victim.freezeProgress++;
			//TODO specal cryo dt effect. change sprite color based on freezeProg?
			break;
		case DamageType.PYRO:
			calcDamage *= (100f - (float)victim.pyroResist.value) / 100f;
			//TODO special pyro dt effect
			break;
		case DamageType.VOID:
			calcDamage *= (100f - (float)victim.voidResist.value) / 100f;
			//TODO special void dt effect
			break;
		}

		bool hitShields = victim.shields > 0f && !ignoreShields;
		if (hitShields)
		{
			//deal damage to the shields
			victim.shields -= calcDamage;
			if (victim.shields <= 0f)
			{
				victim.shields = 0f;
				victim.OnShieldsDown ();
				victim.shieldDelay = victim.shieldDelayMax;
			}
		}
		else
		{
			//deal damage to health
			victim.health -= calcDamage;
			if (victim.health <= 0f)
			{
				victim.health = 0f;
				victim.OnDeath ();
			}
		}

		//combat event hooks
		victim.OnDamageTaken (attacker, damage, calcDamage, dt, hitShields);
		if(attacker != null)
			attacker.OnDamageDealt (victim, damage, calcDamage, dt, hitShields);
	}

	public static void healEntity(Entity e, float healAmount)
	{
		if (healAmount < 0f)
			healAmount = 0f;
		e.health += healAmount;
		if (e.health > e.healthMax)
			e.health = e.healthMax;

		foreach (Status s in e.statuses)
			s.OnHealed (e, healAmount);

		if(e.healed != null)
			e.healed(healAmount);
		
		//TODO heal effect
	}

	/* Instance Methods */
	public void Awake()
	{
		//setup non-editor setable values
		health = healthMax;
		shields = shieldsMax;
		shieldDelay = shieldDelayMax;

		combatTimer = 0f;

		invincible = new Stat (0, 0);
		stunned = new Stat (0, 0);
		rooted = new Stat (0, 0);
		freezeProgress = 0f;

		statuses = new List<Status> ();

		collisonLog = new HashSet<Bullet> ();
	}

	public void Start()
	{
		Entity parEnt = GetComponentInParent<Entity> ();
		Destructable parDes = GetComponentInParent<Destructable> ();

		if (parEnt != null && parEnt != this)
			parEnt.died += OnDeath;
		else if (parDes != null)
			parDes.destructed += OnDeath;
	}

	public float healthPerc { get { return health / healthMax; } }
	public float shieldPerc { get { return shields / shieldsMax; } }


	public Faction getFaction()
	{
		return faction;
	}

	// --- IReapable Methods ---
	public SeedBase reap()
	{
		Seed seed = new Seed (gameObject);

		return seed;
	}
	public void sow(SeedBase s)
	{
		Seed seed = (Seed)s;
		destroyed = seed.destroyed;
		if (destroyed)
		{
			//Entity is destroyed
			Destroy (gameObject);
			return;
		}

		//-SeedBase values-
		transform.position = seed.tPosition;
		transform.rotation = seed.tRotation;

		Rigidbody2D body = GetComponent<Rigidbody2D> ();
		if (body != null)
		{
			body.position = seed.rbPosition;
			body.rotation = seed.rbRotation;
			body.velocity = seed.rbVelocity;
			body.angularVelocity = seed.rbAngVelocity;
		}

		//-Entity-specific values-
		faction = seed.faction;

		//resource pools
		health = seed.health;
		healthMax = seed.healthMax;

		shields = seed.shields;
		shieldsMax = seed.shieldsMax;
		shieldRegen = seed.shieldRegen;
		shieldDelay = seed.shieldDelay;
		shieldDelayMax = seed.shieldDelayMax;

		//stats
		physResist = seed.physResist;
		elecResist = seed.elecResist;
		biolResist = seed.biolResist;
		cryoResist = seed.cryoResist;
		pyroResist = seed.pyroResist;
		voidResist = seed.voidResist;

		movespeed = seed.movespeed;

		//built-in statuses
		invincible = seed.invincible;
		stunned = seed.stunned;
		rooted = seed.rooted;

		freezeProgress = seed.freezeProgress;

		//status list
		statuses = seed.statuses;
		foreach(Status st in statuses)
		{
			st.durationCompleted += removeStatus;
			if (statusAdded != null)
				statusAdded (st);
		}
		
		//ability list
		abilities = seed.abilities;
		foreach (Ability a in abilities)
			if (abilityAdded != null)
				abilityAdded (a);
	}
	public bool destroyed { get; set; }
	public bool ignoreReset() { return !allowReset; }

	// --- Monobehavior Stuff ---
	public void Update()
	{
		//update all statuses
		foreach (Status s in statuses)
			s.updateDuration (this, Time.deltaTime);

		//update all abilities
		foreach (Ability a in abilities)
			a.updateCooldown (Time.deltaTime);

		//update combat timer
		combatTimer -= Time.deltaTime;
		if (combatTimer <= 0f)
			combatTimer = 0f;

		//update freeze progress
		freezeProgress -= Time.deltaTime;
		if (freezeProgress <= 0f)
			freezeProgress = 0f;

		//shield recharge + recharge delay
		shieldDelay -= Time.deltaTime;
		if (shieldDelay <= 0f)
		{
			shieldDelay = 0f;
			shields += (shieldRegen * Time.deltaTime);
			shields = shields > shieldsMax ? shieldsMax : shields;
		}
	}

	// --- Status Handling ---

	// Add a status to the Entity and begin listening for its end
	public void addStatus(Status s)
	{
		Status existing = statuses.Find (delegate(Status obj) { return obj.Equals(s); });
		if (existing != null)
		{
			//a status of this type is already on this Entity
			existing.stack (this, 1);
			return;
		}

		//this status is new to this Entity
		statuses.Add (s);
		s.durationCompleted += removeStatus;
		s.OnApply (this);

		//notify listeners
		if (statusAdded != null)
			statusAdded (s);
	}

	// Either a status naturally ran out, or it is being manually removed
	public void removeStatus(Status s)
	{
		s.OnRevert (this);
		s.durationCompleted -= removeStatus;

		//notify listeners
		if (statusAdded != null)
			statusRemoved (s);
	}

	// --- Ability Handling ---

	// Add an ability to this Entity
	public void addAbility(Ability a)
	{
		//don't allow ability changes if in combat
		if (inCombat())
			return;

		//add the ability and set it to active
		abilities.Add (a);
		a.active = true;

		//notify listeners
		if (abilityAdded != null)
			abilityAdded (a);
	}

	// Remove an ability from this Entity
	public void removeAbility(Ability a)
	{
		//don't allow ability changes if in combat
		if (inCombat())
			return;

		//remove the ability and set it to inactive
		abilities.Remove (a);
		a.active = false;

		//notify listeners
		if (abilityRemoved != null)
			abilityRemoved (a);
	}
	#pragma warning disable 0168
	public void removeAbility(int index)
	{
		try
		{
			removeAbility(abilities[index]);
		}
		catch(IndexOutOfRangeException ioore)
		{
			Debug.LogError ("Attempted to remove a non-existant Ability."); //DEBUG
		}
	}

	// Swap the ability at index for a new ability. Returns the the old ability (null if fails)
	public Ability swapAbility(Ability a, int index)
	{
		if (inCombat())
			return null;

		//swap 'em
		Ability old = null;
		try
		{
			old = abilities [index];
			abilities [index] = a;
			a.active = true;
			old.active = false;
		}
		catch(IndexOutOfRangeException ioore)
		{
			Debug.LogError ("Tried to swap out non-existant Ability"); //DEBUG
			return null;
		}

		if (abilitySwapped != null)
			abilitySwapped (a, old, index);

		return old;
	}
	#pragma warning restore 0168

	// Ability getter
	public Ability getAbility(int index)
	{
		return abilities [index];
	}

	// --- Collision Log Handling ---

	// Add an entry to the collision log
	// Returns false if the entry already exists
	public bool addColLogEntry(Bullet bullet)
	{
		bool isNew = collisonLog.Add (bullet);
		if (isNew)
			bullet.bulletDied += removeColLogEntry;
		return isNew;
	}

	// Called automatically but bullets in the collision log when they die
	private void removeColLogEntry(Bullet bullet)
	{
		bool existed = collisonLog.Remove (bullet);
		if (existed)
			bullet.bulletDied -= removeColLogEntry;
	}

	// --- inCombat, Frozen, Invincible, Stunned, Rooted accessors and modifiers ---
	public bool inCombat()
	{
		return combatTimer > 0f;
	}
	public bool frozen()
	{
		return freezeProgress > (float)cryoResist.value + 50f;
	}
	public bool setInvincible(bool val)
	{
		if (val)
			invincible += 1;
		else
			invincible -= 1;
		return isInvincible ();
	}
	public bool isInvincible()
	{
		return invincible.value > 0;
	}

	public bool setStunned(bool val)
	{
		if (val)
		{
			stunned += 1;

			foreach (Status s in statuses)
				s.OnStunned (this);

			if (wasStunned != null)
				wasStunned ();
		}
		else
			stunned -= 1;
		return isStunned ();
	}
	public bool isStunned()
	{
		return stunned.value > 0;
	}

	public bool setRooted(bool val)
	{
		if (val)
		{
			rooted += 1;

			foreach (Status s in statuses)
				s.OnRooted (this);

			if (wasRooted != null)
				wasRooted ();
		}
		else
			rooted -= 1;
		return isRooted ();
	}
	public bool isRooted()
	{
		return rooted.value > 0;
	}

	// --- Hook Callers ---

	// This Entity took damage
	private void OnDamageTaken(Entity attacker, float rawDamage, float calcDamage, DamageType dt, bool hitShields)
	{
		foreach (Status s in statuses)
			s.OnDamageTaken (this, attacker, rawDamage, calcDamage, dt, hitShields);

		if (tookDamage != null)
			tookDamage (this, attacker, rawDamage, calcDamage, dt, hitShields);
	}

	// This Entity dealt damage
	private void OnDamageDealt(Entity victim, float rawDamage, float calcDamage, DamageType dt, bool hitShields)
	{
		foreach (Status s in statuses)
			s.OnDamageDealt (this, victim, rawDamage, calcDamage, dt, hitShields);

		if (tookDamage != null)
			dealtDamage (victim, this, rawDamage, calcDamage, dt, hitShields);
	}

	// This Entity has died
	public void OnDeath()
	{
		foreach (Status s in statuses)
			s.OnDeath (this);
		
		if (died != null)
			died ();

		GetComponent<RegisteredObject> ().saveDestruction ();
		
		Destroy(gameObject);
	}

	// Shields have fallen to zero
	private void OnShieldsDown()
	{
		foreach (Status s in statuses)
			s.OnShieldsDown (this);

		if (shieldsBroken != null)
			shieldsBroken ();
	}

	// Shields have recharged to full
	private void OnShieldsRecharged()
	{
		foreach (Status s in statuses)
			s.OnShieldsRecharged (this);

		if (shieldsRecharged != null)
			shieldsRecharged ();
	}

	/* Delegates and Events */
	public delegate void StatusChanged(Status s);
	public event StatusChanged statusAdded;
	public event StatusChanged statusRemoved;

	public delegate void AbilityChanged(Ability a);
	public event AbilityChanged abilityAdded;
	public event AbilityChanged abilityRemoved;
	public delegate void AbilitySwap(Ability a, Ability old, int index);
	public event AbilitySwap abilitySwapped;

	public delegate void EntityAttacked(Entity victim, Entity attacker, float rawDamage, float calcDamage, DamageType dt, bool hitShields);
	public event EntityAttacked tookDamage;
	public event EntityAttacked dealtDamage;

	public delegate void EntityGeneric();
	public event EntityGeneric died;
	public event EntityGeneric shieldsBroken;
	public event EntityGeneric shieldsRecharged;
	public event EntityGeneric wasStunned;
	public event EntityGeneric wasRooted;

	public delegate void EntityHealed(float healAmount);
	public event EntityHealed healed;

	/* Inner Classes */
	[Serializable]
	private class Seed : SeedBase
	{
		/* Instance Vars */
		public Faction faction;

		public float health;
		public float healthMax;

		public float shields;
		public float shieldsMax;
		public float shieldRegen;
		public float shieldDelay;
		public float shieldDelayMax;

		public Stat physResist;
		public Stat elecResist;
		public Stat biolResist;
		public Stat cryoResist;
		public Stat pyroResist;
		public Stat voidResist;

		public Stat movespeed;

		public Stat invincible;
		public Stat stunned;
		public Stat rooted;

		public float freezeProgress;

		public List<Status> statuses;
		public List<Ability> abilities;

		/* Constructors */
		public Seed(GameObject subject) : base(subject)
		{
			Entity subInfo = subject.GetComponent<Entity>();

			faction = subInfo.faction;

			health = subInfo.health;
			healthMax = subInfo.healthMax;

			shields = subInfo.shields;
			shieldsMax = subInfo.shieldsMax;
			shieldRegen = subInfo.shieldRegen;
			shieldDelay = subInfo.shieldDelay;
			shieldDelayMax = subInfo.shieldDelayMax;

			physResist = subInfo.physResist;
			elecResist = subInfo.elecResist;
			biolResist = subInfo.biolResist;
			cryoResist = subInfo.cryoResist;
			pyroResist = subInfo.pyroResist;
			voidResist = subInfo.voidResist;

			movespeed = subInfo.movespeed;

			invincible = subInfo.movespeed;
			stunned = subInfo.stunned;
			rooted = subInfo.rooted;

			freezeProgress = subInfo.freezeProgress;

			statuses = subInfo.statuses;
			abilities = subInfo.abilities;
		}
		public Seed(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			faction = (Faction)info.GetInt32("faction");

			health = info.GetSingle("health");
			healthMax = info.GetSingle("healthMax");

			shields = info.GetSingle("shields");
			shieldsMax = info.GetSingle("shieldsMax");
			shieldRegen = info.GetSingle("shieldRegen");
			shieldDelay = info.GetSingle("shieldDelay");
			shieldDelayMax = info.GetSingle("shieldDelayMax");

			physResist = (Stat)info.GetValue("physResist", typeof(Stat));
			elecResist = (Stat)info.GetValue("elecResist", typeof(Stat));
			biolResist = (Stat)info.GetValue("biolResist", typeof(Stat));
			cryoResist = (Stat)info.GetValue("cryoResist", typeof(Stat));
			pyroResist = (Stat)info.GetValue("pyroResist", typeof(Stat));
			voidResist = (Stat)info.GetValue("voidResist", typeof(Stat));

			movespeed = (Stat)info.GetValue("movespeed", typeof(Stat));

			invincible = (Stat)info.GetValue("invincible", typeof(Stat));
			stunned = (Stat)info.GetValue("stunned", typeof(Stat));
			rooted = (Stat)info.GetValue("rooted", typeof(Stat));

			freezeProgress = info.GetSingle("freezeProgress");

			int statusSize = info.GetInt32("statusSize");
			statuses =  new List<Status>();
			for(int i = 0; i < statusSize; i++)
				statuses.Add((Status)info.GetValue("status" + i, typeof(Status)));

			int abilSize = info.GetInt32("abilSize");
			abilities =  new List<Ability>();
			for(int i = 0; i < abilSize; i++)
				abilities.Add((Ability)info.GetValue("abil" + i, typeof(Ability)));
		}

		public override void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData (info, context);

			info.AddValue ("faction", (int)faction);

			info.AddValue ("health", health);
			info.AddValue ("healthMax", healthMax);

			info.AddValue ("shields", shields);
			info.AddValue ("shieldsMax", shieldsMax);
			info.AddValue ("shieldRegen", shieldRegen);
			info.AddValue ("shieldDelay", shieldDelay);
			info.AddValue ("shieldDelayMax", shieldDelayMax);

			info.AddValue ("physResist", physResist);
			info.AddValue ("elecResist", elecResist);
			info.AddValue ("biolResist", biolResist);
			info.AddValue ("cryoResist", cryoResist);
			info.AddValue ("pyroResist", pyroResist);
			info.AddValue ("voidResist", voidResist);

			info.AddValue ("movespeed", movespeed);

			info.AddValue ("invincible", invincible);
			info.AddValue ("stunned", stunned);
			info.AddValue ("rooted", rooted);

			info.AddValue ("freezeProgress", freezeProgress);

			info.AddValue ("statusSize", statuses.Count);
			for (int i = 0; i < statuses.Count; i++)
				info.AddValue ("status" + i, statuses [i]);

			info.AddValue ("abilSize", abilities.Count);
			for (int i = 0; i < abilities.Count; i++)
				info.AddValue ("abil" + i, abilities [i]);
		}
	}
}
