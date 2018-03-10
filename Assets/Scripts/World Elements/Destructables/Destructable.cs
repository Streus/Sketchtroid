using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System;

public class Destructable : MonoBehaviour, IReapable
{
	/* Instance Vars */

	[SerializeField]
	private float health;

	/* Instance Methods */
	public void Start()
	{
		Entity parEnt = GetComponentInParent<Entity> ();
		Destructable parDes = GetComponentInParent<Destructable> ();

		if (parEnt != null)
			parEnt.died += OnDeath;
		else if (parDes != null)
			parDes.destructed += OnDeath;
	}

	public void applyDamage(float amount)
	{
		OnHit (amount);

		health -= amount;
		if (health <= 0f)
			OnDeath ();
	}

	public SeedCollection.Base reap()
	{
		Seed seed = new Seed (gameObject);
		return seed;
	}
	public void sow(SeedCollection.Base s)
	{
		Seed seed = (Seed)s;

		health = seed.health;
	}

	protected virtual void OnHit(float damage)
	{
		
	}

	public virtual void OnDeath()
	{
		if (destructed != null)
			destructed ();

		RegisteredObject.destroy (gameObject);
	}

	public void OnDestroy()
	{
		//create stuff in here and I'll cut you
	}

	/* Events and Delegates */
	public delegate void DestructableDestroyed();
	public event DestructableDestroyed destructed;

	/* Inner classes */
	[Serializable]
	private class Seed : SeedCollection.Base
	{
		/* Instance Vars */
		public float health;

		/* Constructors */
		public Seed(GameObject subject)
		{
			health = subject.GetComponent<Destructable>().health;
		}
		public Seed(SerializationInfo info, StreamingContext context)
		{
			health = info.GetSingle("health");
		}

		public override void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			info.AddValue ("health", health);
		}
	}
}
