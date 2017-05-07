using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
	/* Static Vars */


	/* Instance Vars */

	// A resource pool that is deducted from when taking damage. Death occurs when it reaches 0.
	private float health;
	public float healthMax;

	// A resource pool that is deducted before health. Can regen after a delay.
	private float shields;
	public float shieldsMax;
	public float shieldRegen;
	private float shieldDelay;
	public float shieldDelayMax;

	// Each reduces the damage taken from their respective damage types
	public Stat physResist;
	public Stat elecResist;
	public Stat biolResist;
	public Stat cryoResist;
	public Stat pyroResist;
	public Stat voidResist;

	// The speed at which this Entity can move through the world
	public Stat movespeed;

	// If > 0, this Entity will not take damage
	private Stat invincible;

	// If > 0, this Entity's Controller cannot take any action
	private Stat stunned;

	// If > 0, this Entity cannot move
	private Stat rooted;

	// Used with Cryo damage type
	private Stat freezeProgress;

	// The Statuses currently affecting this Entity
	private List<Status> statuses;

	// The Extensions currently applied to this Entity
	private List<Extension> extensions;

	// The Abilities that this Entity
	private List<Ability> abilities;

	/* Static Methods */
	public static void damageEntity(Entity victim, Entity attacker, float damage, DamageType dt, bool ignoreShields = false, params Status[] s)
	{
		if (damage < 0)
			damage = 0;

		//the actual damage value that will be subtracted from health/shields
		float calcDamage = damage;

		//do special dt-based effects and apply resistances
		switch (dt)
		{
		case DamageType.PHYSICAL:
			calcDamage /= (float)victim.physResist;
			break;
		case DamageType.ELECTRIC:
			calcDamage /= (float)victim.elecResist;
			//TODO special electric dt effect
			break;
		case DamageType.BIO:
			calcDamage /= (float)victim.biolResist;
			//TODO special biological dt effect
			break;
		case DamageType.CRYO:
			calcDamage /= (float)victim.cryoResist;
			//TODO specal cryo dt effect
			break;
		case DamageType.PYRO:
			calcDamage /= (float)victim.pyroResist;
			//TODO special pyro dt effect
			break;
		case DamageType.VOID:
			calcDamage /= (float)victim.voidResist;
			//TODO special void dt effect
			break;
		}

		bool hitShields = victim.shields > 0f;
		if (hitShields)
		{
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
			victim.health -= calcDamage;
			if (victim.health <= 0f)
			{
				victim.health = 0f;
				victim.OnDeath ();
			}
		}

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
		
		//TODO heal effect
	}

	/* Instance Methods */
	public void Awake()
	{
		// Setup defaults
		healthMax = 75f;
		health = healthMax;

		shieldsMax = 25f;
		shields = shieldsMax;
		shieldRegen = 1f;
		shieldDelayMax = 5f;
		shieldDelay = 0f;

		physResist = new Stat (0, 0, 100);
		elecResist = new Stat (0, 0, 100);
		biolResist = new Stat (0, 0, 100);
		cryoResist = new Stat (0, 0, 100);
		pyroResist = new Stat (0, 0, 100);
		voidResist = new Stat (0, 0, 100);

		movespeed = new Stat (5, 0);

		invincible = new Stat (0, 0);
		stunned = new Stat (0, 0);
		rooted = new Stat (0, 0);
		freezeProgress = new Stat (0, 0);

		statuses = new List<Status> ();
		extensions = new List<Extension> (3);
		abilities = new List<Ability> (3);


		// Load values if this Entity has a profile saved
		//TODO
	}

	public void Start()
	{

	}

	public void Update()
	{

	}

	// Invincible, Stunned, Rooted accessors and modifiers
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
			stunned += 1;
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
			rooted += 1;
		else
			rooted -= 1;
		return isRooted ();
	}
	public bool isRooted()
	{
		return rooted.value > 0;
	}

	// --Hook Callers--

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
	private void OnDeath()
	{
		foreach (Status s in statuses)
			s.OnDeath (this);
		
		if (entityDied != null)
			entityDied ();
	}

	// Shields have fallen to zero
	private void OnShieldsDown()
	{
		foreach (Status s in statuses)
			s.OnShieldsDown (this);

		if (shieldsBroken != null)
			shieldsBroken ();
	}

	//TODO add the rest of the hook callers, delegates, and events

	/* Delegates and Events */
	public delegate void EntityAttacked(Entity victim, Entity attacker, float rawDamage, float calcDamage, DamageType dt, bool hitShields);
	public event EntityAttacked tookDamage;
	public event EntityAttacked dealtDamage;

	public delegate void EntityDeath();
	public event EntityDeath entityDied;

	public delegate void EntityShieldsDown();
	public event EntityShieldsDown shieldsBroken;
}
