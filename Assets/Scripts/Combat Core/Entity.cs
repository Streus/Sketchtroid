using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization;
using System;

public sealed class Entity : MonoBehaviour, IReapable
{
	#region STATIC_VARS

	private const float COMBAT_TIMER_MAX = 5f; // 5 seconds
	#endregion

	#region INSTANCE_VARS

	// The faction this Entity belongs to. Entities of the same faction cannot hurt eachother
	[SerializeField]
	private Faction faction;

	// A resource pool that is deducted from when taking damage. Death occurs when it reaches 0.
	[SerializeField]
	private float health;
	public float healthMax = 75f;

	// A resource pool that is deducted before health. Can regen after a delay.
	[SerializeField]
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

	// The default damage type this Entity will deal
	[SerializeField]
	private DamageType _defaultDT = DamageType.PHYSICAL;
	public DamageType DefaultDT
	{
		get { return _defaultDT; }
		set
		{
			_defaultDT = value;
			if (damageTypeChanged != null)
				damageTypeChanged (value);
		}
	}

	// The speed at which this Entity can move through the world
	public Stat movespeed = new Stat(0, 0, 25);

	// If > 0, this Entity is in combat
	private float combatTimer;

	// If > 0, this Entity will not take damage
	private int invincible;

	// If > 0, this Entity's Controller cannot take any action
	private int stunned;

	// If > 0, this Entity cannot move
	private int rooted;

	// Used with Cryo damage type
	private float freezeProgress;

	// The Statuses currently affecting this Entity
	[SerializeField]
	private Dictionary<string, Status> statuses;

	// The Abilities that this Entity
	private Ability[] abilities;
	private int abilSize;

	// The Bullets with which this Entity and its netowrk have collided
	private HashSet<Bullet> collisonLog;

	#endregion

	#region STATIC_METHODS
	public static void DamageEntity(Entity victim, Entity attacker, float damage, DamageType dt, bool ignoreShields = false, params Status[] s)
	{
		//everyone is in combat
		victim.combatTimer = COMBAT_TIMER_MAX;
		if (attacker != null)
			attacker.combatTimer = COMBAT_TIMER_MAX;

		//don't deal negative damage; that's healing
		if (damage < 0)
			damage = 0;

		//the actual damage value that will be subtracted from health/shields
		float calcDamage = damage;

		bool hitShields = false;

		//victim is invincible, do nothing
		if (!victim.Invincible)
		{
			//do special dt-based effects and apply resistances
			switch (dt)
			{
			case DamageType.PHYSICAL:
				calcDamage *= (100f - (float)victim.physResist.Value) / 100f;
				break;
			case DamageType.ELECTRIC:
				calcDamage *= (100f - (float)victim.elecResist.Value) / 100f;
				//TODO special electric dt effect
				break;
			case DamageType.BIO:
				calcDamage *= (100f - (float)victim.biolResist.Value) / 100f;
				//TODO special biological dt effect
				break;
			case DamageType.CRYO:
				calcDamage *= (100f - (float)victim.cryoResist.Value) / 100f;
				victim.freezeProgress++;
				//TODO specal cryo dt effect. change sprite color based on freezeProg?
				break;
			case DamageType.PYRO:
				calcDamage *= (100f - (float)victim.pyroResist.Value) / 100f;
				//TODO special pyro dt effect
				break;
			case DamageType.VOID:
				calcDamage *= (100f - (float)victim.voidResist.Value) / 100f;
				//TODO special void dt effect
				break;
			}

			hitShields = victim.shields > 0f && !ignoreShields;
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
		}
		else
		{
			calcDamage = damage = 0f;
		}

		//combat event hooks
		victim.OnDamageTaken (attacker, damage, calcDamage, dt, hitShields);
		if(attacker != null)
			attacker.OnDamageDealt (victim, damage, calcDamage, dt, hitShields);
	}

	public static void HealEntity(Entity e, float healAmount)
	{
		if (healAmount < 0f)
			healAmount = 0f;
		e.health += healAmount;
		if (e.health > e.healthMax)
			e.health = e.healthMax;

		foreach (Status s in e.statuses.Values)
			s.onHealed (e, healAmount);

		if(e.healed != null)
			e.healed(healAmount);
		
		//TODO heal effect
	}
	#endregion

	#region INSTANCE_METHODS

	public void Awake()
	{
		//setup non-editor setable values
		health = healthMax;
		shields = shieldsMax;
		shieldDelay = shieldDelayMax;

		combatTimer = 0f;

		invincible = 0;
		stunned = 0;
		rooted = 0;
		freezeProgress = 0f;

		statuses = new Dictionary<string, Status>();
		abilities = new Ability[3];
		abilSize = 0;

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

	public float HealthPerc { get { return health / healthMax; } }
	public float ShieldPerc { get { return shields / shieldsMax; } }


	public Faction GetFaction()
	{
		return faction;
	}

	// --- IReapable Methods ---
	public SeedCollection.Base Reap()
	{
		Seed seed = new Seed (gameObject);

		return seed;
	}
	public void Sow(SeedCollection.Base s)
	{
		if (s == null)
			return;

		Seed seed = (Seed)s;

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

		DefaultDT = seed.defaultDT;

		movespeed = seed.movespeed;

		//built-in statuses
		invincible = seed.invincible;
		stunned = seed.stunned;
		rooted = seed.rooted;

		freezeProgress = seed.freezeProgress;

		//status list
		statuses = seed.statuses;
		foreach(Status st in statuses.Values)
		{
			if (statusAdded != null)
				statusAdded (st);
		}
		
		//ability list
		abilities = seed.abilities;
		foreach (Ability a in abilities)
			if (abilityAdded != null)
				abilityAdded (a);
	}

	// --- Monobehavior Stuff ---
	public void Update()
	{
		//update all statuses
		List<string> removeList = new List<string> ();
		foreach (Status s in statuses.Values)
		{
			if (s.UpdateDuration (this, Time.deltaTime))
				removeList.Add (s.name);
		}

		//remove statuses that have finished their durations
		for (int i = 0; i < removeList.Count; i++)
		{
			Status s;
			if (statuses.TryGetValue (removeList[i], out s))
			{
				RemoveStatus (s);
			}
		}

		//update all abilities
		for (int i = 0; i < abilities.Length; i++)
		{
			if (abilities [i] != null)
				abilities [i].UpdateCooldown (Time.deltaTime);
		}

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

	#region STATUS_HANDLING

	// Add a status to the Entity and begin listening for its end
	public void AddStatus(Status s)
	{
		Status existing;
		if (statuses.TryGetValue (s.name, out existing))
		{
			existing.Stack (this, 1);
			return;
		}

		//this status is new to this Entity
		statuses.Add (s.name, s);
		s.OnApply (this);

		//notify listeners
		if (statusAdded != null)
			statusAdded (s);
	}

	// Either a status naturally ran out, or it is being manually removed
	public void RemoveStatus(Status s)
	{
		s.OnRevert (this);
		statuses.Remove (s.name);

		//notify listeners
		if (statusRemoved != null)
			statusRemoved (s);
	}

	// Check for a specific status in this Entity's status list
	public bool HasStatus(string name)
	{
		return statuses.ContainsKey (name);
	}

	public bool HasStatus(Status s)
	{
		return HasStatus (s.name);
	}

	// Get an IEnumerable so that the list can be iterated over, but not changed
	// Casting totally isn't a thing, right?
	public IEnumerable GetStatusList()
	{
		return statuses;
	}
	#endregion

	#region ABILITY_HANDLING

	// Add an ability to this Entity
	public bool AddAbility(Ability a, int index = -1)
	{
		//don't allow ability changes if in combat
		//don't allow null insertions
		//don't allow active ability additions
		if (InCombat() || a == null || a.Active)
			return false;

		//add the ability and set it to active
		if (index == -1)
		{
			abilities [abilSize++] = a;
			a.Active = true;

			if (abilSize >= abilities.Length)
				ResizeAbilities ();
		}
		else if (index >= 0 && index < abilities.Length)
		{
			if (abilities [index] != null && abilityRemoved != null)
				abilityRemoved (abilities [index], index);
			abilities [index] = a;
			a.Active = true;
		}
		else
			return false;

		//notify listeners
		if (abilityAdded != null)
			abilityAdded (a, index);

		return true;
	}
	private void ResizeAbilities()
	{
		Ability[] temp = abilities;
		abilities = new Ability[temp.Length * 2];
		for (int i = 0; i < temp.Length; i++)
			abilities [i] = temp [i];
	}

	// Remove an ability from this Entity
	public bool RemoveAbility(Ability a)
	{
		//don't allow ability changes if in combat
		if (InCombat())
			return false;

		//remove the ability and set it to inactive
		for (int i = 0; i < abilities.Length; i++)
		{
			//look for equal reference
			if (a == abilities [i])
				return RemoveAbility (i);
		}
		return false;
	}
	public bool RemoveAbility(int index)
	{
		if (InCombat ())
			return false;

		Ability removed = null;

		try
		{
			if(abilities[index] == null)
				return false;

			removed = abilities[index];
			abilities[index] = null;
			removed.Active = false;

			if (abilityRemoved != null)
				abilityRemoved (removed, index);
		}
		catch(IndexOutOfRangeException)
		{
			Debug.LogError ("Attempted to remove a non-existant Ability. Index: " + index); //DEBUG
			return false;
		}

		return true;
	}

	// Swap the ability at index for a new ability. Returns the the old ability (null if fails)
	public Ability SwapAbility(Ability a, int index)
	{
		if (InCombat() || a == null)
			return null;

		//swap 'em
		Ability old = null;
		try
		{
			old = abilities [index];
			abilities [index] = a;
			if(a != null)
				a.Active = true;
			if(old != null)
				old.Active = false;
		}
		catch(IndexOutOfRangeException)
		{
			Debug.LogError ("Tried to swap out non-existant Ability. Index: " + index); //DEBUG
			return null;
		}

		if (abilitySwapped != null)
			abilitySwapped (a, old, index);

		return old;
	}

	// Ability getter
	public Ability GetAbility(int index)
	{
		if (index < 0 || index >= abilities.Length)
			return null;
		return abilities [index];
	}

	// For externally looping through the ability list
	public int AbilityCount { get { return abilSize; } }
	public int AbilityCap { get { return abilities.Length; } }
	#endregion

	#region COLLISION_LOG_HANDLING

	// Add an entry to the collision log
	// Returns false if the entry already exists
	public bool AddColLogEntry(Bullet bullet)
	{
		bool isNew = collisonLog.Add (bullet);
		if (isNew)
			bullet.bulletDied += RemoveColLogEntry;
		return isNew;
	}

	// Called automatically by bullets in the collision log when they die
	private void RemoveColLogEntry(Bullet bullet)
	{
		bool existed = collisonLog.Remove (bullet);
		if (existed)
			bullet.bulletDied -= RemoveColLogEntry;
	}
	#endregion

	#region BUILTIN_STATUSES

	public bool InCombat()
	{
		return combatTimer > 0f;
	}
	public bool Frozen
	{
		get { return freezeProgress > (cryoResist.Value + 50f); }
	}
	
	public bool Invincible
	{
		get { return invincible >= 0; }
		set
		{
			invincible += value ? 1 : -1;
		}
	}

	public bool Stunned
	{
		get { return stunned >= 0; }
		set
		{
			int startStunned = stunned;
			stunned += value ? 1 : -1;
			if (startStunned <= 0 && stunned > 0 && wasStunned != null)
				wasStunned ();
		}
	}

	public bool Rooted
	{
		get { return rooted >= 0; }
		set
		{
			int startRooted = rooted;
			rooted += value ? 1 : -1;
			if (startRooted <= 0 && rooted > 0 && wasRooted != null) 
				wasRooted ();
		}
	}

	#endregion

	#region HOOKS

	// This Entity took damage
	private void OnDamageTaken(Entity attacker, float rawDamage, float calcDamage, DamageType dt, bool hitShields)
	{
		foreach (Status s in statuses.Values)
			s.OnDamageTaken (this, attacker, rawDamage, calcDamage, dt, hitShields);

		if (tookDamage != null)
			tookDamage (this, attacker, rawDamage, calcDamage, dt, hitShields);
	}

	// This Entity dealt damage
	private void OnDamageDealt(Entity victim, float rawDamage, float calcDamage, DamageType dt, bool hitShields)
	{
		foreach (Status s in statuses.Values)
			s.OnDamageDealt (this, victim, rawDamage, calcDamage, dt, hitShields);

		if (tookDamage != null)
			dealtDamage (victim, this, rawDamage, calcDamage, dt, hitShields);
	}

	// This Entity has died
	public void OnDeath()
	{
		foreach (Status s in statuses.Values)
			s.OnDeath (this);
		
		if (died != null)
			died ();

		RegisteredObject.Destroy (gameObject);
	}

	// Shields have fallen to zero
	private void OnShieldsDown()
	{
		foreach (Status s in statuses.Values)
			s.OnShieldsDown (this);

		if (shieldsBroken != null)
			shieldsBroken ();
	}

	// Shields have recharged to full
	private void OnShieldsRecharged()
	{
		foreach (Status s in statuses.Values)
			s.OnShieldsRecharged (this);

		if (shieldsRecharged != null)
			shieldsRecharged ();
	}
	#endregion
	#endregion

	#region INTERNAL_TYPES

	/* Delegates and Events */
	public delegate void ChangeDamageType(DamageType dt);
	public event ChangeDamageType damageTypeChanged;

	public delegate void StatusChanged(Status s);
	public event StatusChanged statusAdded;
	public event StatusChanged statusRemoved;

	public delegate void AbilityChanged(Ability a, int index = -1);
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
	public class Seed : SeedCollection.Base
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

		public DamageType defaultDT;

		public Stat movespeed;

		public float combatTimer;

		public int invincible;
		public int stunned;
		public int rooted;

		public float freezeProgress;

		public Dictionary<string, Status> statuses;
		public Ability[] abilities;

		/* Constructors */
		public Seed(GameObject subject)
		{
			Entity subInfo = subject.GetComponent<Entity>();
			if(subInfo == null)
				return;

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

			defaultDT = subInfo._defaultDT;

			movespeed = subInfo.movespeed;

			combatTimer = subInfo.combatTimer;

			invincible = subInfo.invincible;
			stunned = subInfo.stunned;
			rooted = subInfo.rooted;

			freezeProgress = subInfo.freezeProgress;

			statuses = subInfo.statuses;
			abilities = subInfo.abilities;
		}
		public Seed(SerializationInfo info, StreamingContext context)
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

			defaultDT = (DamageType)info.GetInt32("defaultDT");

			movespeed = (Stat)info.GetValue("movespeed", typeof(Stat));

			combatTimer = info.GetSingle("combatTimer");

			invincible = info.GetInt32("invincible");
			stunned = info.GetInt32 ("stunned");
			rooted = info.GetInt32 ("rooted");

			freezeProgress = info.GetSingle("freezeProgress");

			statuses = (Dictionary<string, Status>)info.GetValue ("statuses", typeof (Dictionary<string, Status>));

			int abilSize = info.GetInt32("abilSize");
			abilities = new Ability[abilSize];
			for(int i = 0; i < abilSize; i++)
				abilities[i] = ((Ability)info.GetValue("abil" + i, typeof(Ability)));
		}

		public override void GetObjectData (SerializationInfo info, StreamingContext context)
		{
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

			info.AddValue ("defaultDT", (int)defaultDT);

			info.AddValue ("movespeed", movespeed);

			info.AddValue ("combatTimer", combatTimer);

			info.AddValue ("invincible", invincible);
			info.AddValue ("stunned", stunned);
			info.AddValue ("rooted", rooted);

			info.AddValue ("freezeProgress", freezeProgress);

			info.AddValue ("statuses", statuses);

			info.AddValue ("abilSize", abilities.Length);
			for (int i = 0; i < abilities.Length; i++)
				info.AddValue ("abil" + i, abilities [i]);
		}
	}
	#endregion
}
