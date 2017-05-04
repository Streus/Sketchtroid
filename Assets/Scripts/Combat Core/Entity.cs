using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
	/* Static Vars */


	/* Instance Vars */

	// A resource pool that is deducted from when taking damage. Death occurs when it reaches 0.
	private int health;
	public Stat healthMax;

	// A resource pool that is deducted before health. Can regen after a delay.
	private int shields;
	public Stat shieldsMax;
	public int shieldRegen;
	private float shieldDelay;
	public Stat shieldDelayMax;

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
	public static void damageEntity(Entity e, int damage, DamageType dt, bool ignoreShields = false, params Status[] s)
	{
		if (damage < 0)
			damage = 0;

		//TODO damage method
	}

	public static void healEntity(Entity e, int healAmount)
	{
		if (healAmount < 0)
			healAmount = 0;
		e.health += healAmount;
		if (e.health > e.healthMax.value)
			e.health = e.healthMax.value;

		//TODO heal effect
	}

	/* Instance Methods */
	public void Awake()
	{
		// Setup defaults
		healthMax = new Stat(75, 0);
		health = healthMax.value;

		shieldsMax = new Stat (25, 0);
		shields = shieldsMax.value;
		shieldRegen = 1;
		shieldDelayMax = new Stat (5, 0);
		shieldDelay = shieldDelayMax.value;

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


}
