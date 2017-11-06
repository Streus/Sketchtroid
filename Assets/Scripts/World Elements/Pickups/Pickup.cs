using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : MonoBehaviour, IReapable
{
	/* Instance Vars */


	/* Instance Methods */

	// ---IReapable Methods---
	public SeedBase reap()
	{
		SeedBase s = new SeedBase (gameObject);
		return s;
	}

	public void sow(SeedBase s)
	{
		s.defaultSow (gameObject);
	}

	public bool ignoreReset()
	{
		return true;
	}

	// Do something to an Entity that collides with this pickup
	protected abstract void apply (Entity e);

	public void OnTriggerEnter2D(Collider2D col)
	{
		Player p = col.GetComponent<Player> ();
		if (p != null)
		{
			apply (p.data);
			//TODO play pickup sound?
			RegisteredObject.destroy(gameObject);
		}
	}
}
