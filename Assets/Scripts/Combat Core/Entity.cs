using System.Collections;
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

	// The Extensions currently applied to this Entity
	[SerializeField]
	private List<Extension> extensions;

	// The Abilities that this Entity
	[SerializeField]
	private List<Ability> abilities;

	// The Bullets with which this Entity and its netowrk have collided
	private HashSet<Bullet> collisonLog;

	/* Static Methods */
	public static void damageEntity(Entity victim, Entity attacker, float damage, DamageType dt, bool ignoreShields = false, params Status[] s)
	{
		//everyone is in combat
		victim.combatTimer = attacker.combatTimer = COMBAT_TIMER_MAX;

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

		//initialize Extensions
		foreach (Extension e in extensions)
			e.init (this);
	}

	public Faction getFaction()
	{
		return faction;
	}

	// --- IReapable Methods ---
	public ISerializable reap()
	{
		Seed seed = new Seed (gameObject);

		//TODO Entity reap

		return seed;
	}
	public void sow(ISerializable s)
	{
		Seed seed = (Seed)s;
		destroyed = seed.destroyed;
		if (destroyed)
		{
			//Entity is destroyed
			Destroy (gameObject);
			return;
		}

		//Entity is not destroyed, load up values
		//TODO Entity sow
	}
	public bool destroyed { get; set; }

	// --- Monobehavior Stuff ---
	public void Start()
	{
		
	}

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

	// --- Extension Handling ---

	// Add an extension to this Entity
	public void addExtension(Extension e)
	{
		extensions.Add (e);
		e.init (this);
	}

	// Remove an extension from this Entity
	public void removeExtension(Extension e)
	{
		extensions.Remove (e);
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
		destroyed = true;

		foreach (Status s in statuses)
			s.OnDeath (this);

		foreach (Extension e in extensions)
			e.cleanup ();
		extensions.Clear ();
		
		if (died != null)
			died ();
		
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

	//TODO add the rest of the hook callers, delegates, and events

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
	private class Seed : SeedBase //TODO Entity.Seed class
	{
		/* Instance Vars */


		/* Constructors */
		public Seed(GameObject subject) : base(subject)
		{

		}
		public Seed(SerializationInfo info, StreamingContext context) : base(info, context)
		{

		}

		public override void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData (info, context);


		}
	}
}
