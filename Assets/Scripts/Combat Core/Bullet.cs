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
	public void Awake()
	{
		colbody = GetComponent<Collider2D> ();
		physbody = GetComponent<Rigidbody2D> ();

		physbody.drag = 0f;
		physbody.AddForce (transform.up * movespeed, ForceMode2D.Impulse);
	}

	public void Update()
	{
		duration -= Time.deltaTime;
		if (duration <= 0f)
		{

		}
	}

	public void FixedUpdate()
	{

	}

	public void LateUpdate()
	{

	}

	public void OnTriggerEnter2D (Collider2D col)
	{
		
	}

	public void OnHit(Collider2D col, Entity hit)
	{

	}

	public void OnDeath()
	{


		//finish up by destroying this bullet
		Destroy(gameObject);
	}

	// Create stuff in here and I'll knife you
	public void OnDestroy()
	{

	}
}

/* Threw this here because indiv. file for it is a waste */
public enum Faction
{
	NEUTRAL,
	PLAYER,
	ENEMY_1,
	ENEMY_2
}

/* Same story */
public enum DamageType
{
	PHYSICAL,
	ELECTRIC,
	BIO,
	CRYO,
	PYRO,
	VOID
}