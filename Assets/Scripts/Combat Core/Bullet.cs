﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction
{
	NEUTRAL,
	PLAYER,
	ENEMY_1,
	ENEMY_2
}

public enum DamageType
{
	NONE,
	PHYSICAL,
	ELECTRIC,
	BIO,
	CRYO,
	PYRO,
	VOID
}

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public sealed class Bullet : MonoBehaviour
{
	#region STATIC_VARS
	private const string PREF_DIR = "core";

	public static Color DamageTypeToColor(DamageType type)
	{
		switch (type)
		{
		case DamageType.PHYSICAL:
			return Color.gray;
		case DamageType.ELECTRIC:
			return new Color (1f, 0.7f, 0f);
		case DamageType.BIO:
			return new Color (0.06f, 0.8f, 0f);
		case DamageType.CRYO:
			return new Color (0f, 0.87f, 0.87f);
		case DamageType.PYRO:
			return new Color (1f, 0f, 0f);
		case DamageType.VOID:
			return new Color (0.39f, 0f, 1f);
		default:
			return Color.white;
		}
	}
	#endregion

	#region INSTANCE_VARS

	[SerializeField]
	private BulletBehavior behavior;

	// The damage this bullet will do when it collides with an Entity of another faction
	[SerializeField]
	private float damage = 1f;

	// The type of damage this bullet deals
	private DamageType _dt;
	public DamageType DamageType
	{
		get { return _dt; }
		set
		{
			_dt = value;
			GetComponent<SpriteRenderer> ().color = DamageTypeToColor (_dt);
		}
	}

	// The speed at which this bullet will begin traveling at instantiation
	[SerializeField]
	private float movespeed = 1f;

	// The faction of this bullet
	[SerializeField]
	private Faction faction;

	// Whether this bullet will be destroyed in a collision event
	[SerializeField]
	private bool destroyOnHit = true;

	// How long this bullet will exist without colliding with anything
	[SerializeField]
	private float duration;
	private float initDuration;

	// The Entity that shot this bullet
	private Entity source;

	// References to important components
	private Collider2D colbody;
	private Rigidbody2D physbody;

	// Bullet death event
	public event BulletDeath bulletDied;
	#endregion

	#region STATIC_METHODS

	// Create a new bullet instance
	public static Bullet Create(GameObject bullet, Entity source, Faction faction = Faction.NEUTRAL)
	{
		GameObject inst = (GameObject)Instantiate (bullet, source.transform.position, source.transform.rotation);
		Bullet b = inst.GetComponent<Bullet> ();
		b.faction = faction;
		b.source = source;
		return b;
	}
	public static Bullet Create(string prefabName, Entity source, Faction faction = Faction.NEUTRAL)
	{
		GameObject go = ABU.LoadAsset<GameObject> (PREF_DIR, prefabName);
		return Create (go, source, faction);
	}
	public static Bullet create(string prefabName, Entity source, DamageType damageType, Faction faction = Faction.NEUTRAL)
	{
		Bullet bullet = Create (prefabName, source, faction);
		bullet.DamageType = damageType;
		return bullet;
	}
	#endregion

	#region INSTANCE_METHODS

	public void Awake()
	{
		colbody = GetComponent<Collider2D> ();
		physbody = GetComponent<Rigidbody2D> ();

		initDuration = duration;

		if (behavior == null)
			Debug.LogError (name + " has an unset behavior!"); //DEBUG unset bullet behavior

		physbody.drag = 0f;
		physbody.AddForce (transform.up * movespeed, ForceMode2D.Impulse);
	}

	public void Start()
	{
		if (behavior != null)
			behavior.OnStart (this);
	}

	public void Update()
	{
		if (!physbody.simulated)
			return;

		duration -= Time.deltaTime;
		if (duration <= 0f)
		{
			OnDeath ();
		}

		if (behavior != null)
			behavior.OnUpdate (this);
	}

	public void FixedUpdate()
	{
		if (behavior != null)
			behavior.OnFixedUpdate (this);
	}

	public void LateUpdate()
	{
		if (behavior != null)
			behavior.OnLateUpdate (this);
	}

	public void OnTriggerEnter2D (Collider2D col)
	{
		Entity e = col.GetComponent<Entity> ();
		CollisionRelay relay = col.GetComponent<CollisionRelay> ();
		CircuitNode i = null; //TODO bullet activated circuit node
		Destructable d = col.GetComponent<Destructable> ();

		if (relay != null && e == null)
		{
			e = relay.LogCollision (this);
		}

		if (e != null)
		{
			if (faction != e.GetFaction ())
			{
				Entity.DamageEntity (e, source, damage, DamageType);
				OnHit (col, e);
				if (destroyOnHit)
					OnDeath ();
			}
		}
		else if (i != null)
		{
			OnHit (col);
//			i.OnInteract (damageType);
		}
		else if (d != null)
		{
			OnHit (col);
			d.ApplyDamage (damage);
		}
		else if (col.tag == "Indes")
		{
			OnHit (col);
			OnDeath ();
		}
	}

	private void OnHit(Collider2D col, Entity hit)
	{
		if (behavior != null)
			behavior.OnHit (this, col, hit);
	}
	private void OnHit(Collider2D col)
	{
		OnHit (col, null);
	}

	public void OnDeath()
	{
		if (behavior != null)
			behavior.OnDeath (this);

		//finish up by destroying this bullet
		Destroy(gameObject);
	}

	// Create stuff in here and I'll knife you
	public void OnDestroy()
	{
		// Call the bulletDied event, which most of the time will remove this bullet
		//from any collision logs it might be in
		if (bulletDied != null)
			bulletDied (this);
	}

	#region GETTERS_SETTERS

	public float GetMovespeed()
	{
		return movespeed;
	}

	public Entity GetSource()
	{
		return source;
	}

	public float GetDuration()
	{
		return duration;
	}

	public void ResetDuration()
	{
		duration = initDuration;
	}

	public Collider2D GetColBody()
	{
		return colbody;
	}

	public Rigidbody2D GetPhysBody()
	{
		return physbody;
	}
	#endregion
	#endregion

	#region INTERNAL_TYPES

	// Event for Entities to listen to so they can cull their collison logs
	public delegate void BulletDeath(Bullet corpse);
	#endregion
}
