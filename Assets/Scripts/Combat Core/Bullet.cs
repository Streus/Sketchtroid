using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public sealed class Bullet : MonoBehaviour
{
	/* Static Vars */
	public static Color damageTypeToColor(DamageType type)
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

	/* Instance Vars */

	[SerializeField]
	private BulletBehavior behavior;

	// The damage this bullet will do when it collides with an Entity of another faction
	[SerializeField]
	private float damage;

	// The type of damage this bullet deals
	private DamageType _dt;
	public DamageType damageType
	{
		get { return _dt; }
		set
		{
			_dt = value;
			GetComponent<SpriteRenderer> ().color = damageTypeToColor (_dt);
		}
	}

	// The speed at which this bullet will begin traveling at instantiation
	[SerializeField]
	private int movespeed;

	// The faction of this bullet
	[SerializeField]
	private Faction faction;

	// Whether this bullet will be destroyed in a collision event
	[SerializeField]
	private bool destroyOnHit;

	// How long this bullet will exist without colliding with anything
	[SerializeField]
	private float duration;

	// The Entity that shot this bullet
	private Entity source;

	// References to important components
	private Collider2D colbody;
	private Rigidbody2D physbody;

	/* Static Methods */

	// Create a new bullet instance
	public static Bullet create(GameObject bullet, Entity source, Faction faction = Faction.NEUTRAL)
	{
		GameObject inst = (GameObject)Instantiate (bullet, source.transform.position, source.transform.rotation);
		Bullet b = inst.GetComponent<Bullet> ();
		b.faction = faction;
		b.source = source;
		return b;
	}
	public static Bullet create(string prefabName, Entity source, Faction faction = Faction.NEUTRAL)
	{
		GameObject go = Resources.Load<GameObject> ("Prefabs/Bullets/" + prefabName);
		return create (go, source, faction);

	}
	public static Bullet create(string prefabName, Entity source, DamageType damageType, Faction faction = Faction.NEUTRAL)
	{
		Bullet bullet = create (prefabName, source, faction);
		bullet.damageType = damageType;
		return bullet;
	}

	/* Instance Methods */
	public void Awake()
	{
		colbody = GetComponent<Collider2D> ();
		physbody = GetComponent<Rigidbody2D> ();

		if (behavior == null)
			Debug.LogError (name + " has an unset behavior!"); //DEBUG unset bullet behavior

		physbody.drag = 0f;
		physbody.AddForce (transform.up * movespeed, ForceMode2D.Impulse);
	}

	public void Start()
	{
		if (behavior != null)
			behavior.start (this);
	}

	public void Update()
	{
		duration -= Time.deltaTime;
		if (duration <= 0f)
		{
			OnDeath ();
		}

		if (behavior != null)
			behavior.update (this);
	}

	public void FixedUpdate()
	{
		if (behavior != null)
			behavior.fixedUpdate (this);
	}

	public void LateUpdate()
	{
		if (behavior != null)
			behavior.lateUpdate (this);
	}

	public void OnTriggerEnter2D (Collider2D col)
	{
		Entity e = col.GetComponent<Entity> ();
		CollisionRelay relay = col.GetComponent<CollisionRelay> ();
		Interactable i = col.GetComponent<Interactable> ();
		Destructable d = col.GetComponent<Destructable> ();

		if (relay != null && e == null)
		{
			e = relay.logCollision (this);
		}

		if (e != null)
		{
			if (faction != e.getFaction ())
			{
				Entity.damageEntity (e, source, damage, damageType);
				OnHit (col, e);
				if (destroyOnHit)
					OnDeath ();
			}
		}
		else if (i != null)
		{
			OnHit (col);
			i.OnInteract (damageType);
		}
		else if (d != null)
		{
			OnHit (col);
			d.damage (damage);
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
			behavior.onHit (this, col, hit);
	}
	private void OnHit(Collider2D col)
	{
		OnHit (col, null);
	}

	public void OnDeath()
	{
		if (behavior != null)
			behavior.onDeath (this);

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

	// Event for Entities to listen to so they can cull their collison logs
	public delegate void BulletDeath(Bullet corpse);
	public event BulletDeath bulletDied;
}

/* Threw this here because indiv. file for it is a waste */
public enum Faction
{
	NEUTRAL,
	PLAYER,
	ENEMY_1,
	ENEMY_2
}

/* Same story as above */
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