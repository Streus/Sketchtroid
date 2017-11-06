using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System;

public class Destructable : MonoBehaviour, IReapable
{
	/* Instance Vars */
	[SerializeField]
	private bool allowReset = true;

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

	public SeedBase reap()
	{
		Seed seed = new Seed (gameObject);
		return seed;
	}
	public void sow(SeedBase s)
	{
		Seed seed = (Seed)s;

		//sow values
		seed.defaultSow(gameObject);

		health = seed.health;
	}
	public bool ignoreReset() { return !allowReset; }

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
	private class Seed : SeedBase
	{
		/* Instance Vars */
		public float health;

		/* Constructors */
		public Seed(GameObject subject) : base(subject)
		{
			health = subject.GetComponent<Destructable>().health;
		}
		public Seed(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			health = info.GetSingle("health");
		}

		public override void GetObjectData (SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData (info, context);

			info.AddValue ("health", health);
		}
	}
}
