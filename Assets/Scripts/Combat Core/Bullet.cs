using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
	/* Static Vars */


	/* Instance Vars */

	// The damage this bullet will do when it collides with an Entity of another faction
	[SerializeField]
	private float damage;

	// The type of damage this bullet deals
	[SerializeField]
	private DamageType damageType;

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
	public virtual void Awake()
	{
		colbody = GetComponent<Collider2D> ();
		physbody = GetComponent<Rigidbody2D> ();

		physbody.drag = 0f;
		physbody.AddForce (transform.up * movespeed, ForceMode2D.Impulse);
	}

	public virtual void Start()
	{

	}

	public virtual void Update()
	{
		duration -= Time.deltaTime;
		if (duration <= 0f)
		{

		}
	}

	public virtual void FixedUpdate()
	{

	}

	public virtual void LateUpdate()
	{

	}

	public void OnTriggerEnter2D (Collider2D col)
	{
		Entity e = col.GetComponent<Entity> ();
		CollisionRelay relay = col.GetComponent<CollisionRelay> ();
		IInteractable i = col.GetComponent<IInteractable> ();
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
				OnEntHit (col, e);
				if (destroyOnHit)
					OnDeath ();
			}
		}
		else if (i != null)
		{
			OnHit (col);
			if (i.getKeyType () == damageType && i.interactable)
			{
				i.OnInteract ();
			}
		}
		else if (d != null)
		{
			OnHit (col);
			d.damage (damage);
		}
	}

	// Collision with an indestructable target
	public void OnCollisionEnter2D(Collision2D col)
	{
		OnHit (col.collider);

		IInteractable i = col.collider.GetComponent<IInteractable> ();
		if (i != null)
		{
			if (i.getKeyType () == damageType && i.interactable)
			{
				i.OnInteract ();
			}
		}
		OnDeath ();
	}

	protected virtual void OnEntHit(Collider2D col, Entity hit)
	{

	}
	protected void OnHit(Collider2D col)
	{
		OnEntHit (col, null);
	}

	public virtual void OnDeath()
	{
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