using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

public class Destructable : MonoBehaviour, IReapable
{
	/* Instance Vars */
	[SerializeField]
	private float health;

	/* Instance Methods */
	public void damage(float amount)
	{
		OnHit (amount);

		health -= amount;
		if (health <= 0f)
			OnDestroy ();
	}

	public ISerializable reap()
	{
		Seed seed = new Seed (gameObject);
		return seed;
	}
	public void sow(ISerializable s)
	{
		Seed seed = (Seed)s;
		destroyed = seed.destroyed;
		if (destroyed)
		{
			//should be destroyed
			Destroy (gameObject);
			return;
		}

		//sow values
		health = seed.health;

		transform.position = seed.tPosition;
		transform.rotation = seed.tRotation;

		Rigidbody2D rb = GetComponent<Rigidbody2D> ();
		if (rb != null)
		{
			rb.position = seed.rbPosition;
			rb.rotation = seed.rbRotation;
			rb.velocity = seed.rbVelocity;
			rb.angularVelocity = seed.rbAngVelocity;
		}
	}
	public bool destroyed { get; set; }

	protected virtual void OnHit(float damage)
	{

	}

	public virtual void OnDestroy()
	{
		if (destructed != null)
			destructed ();

		Destroy (gameObject);
	}

	/* Events and Delegates */
	public delegate void DestructableDestroyed();
	public event DestructableDestroyed destructed;

	/* Inner classes */
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
