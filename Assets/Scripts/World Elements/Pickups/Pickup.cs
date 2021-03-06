﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Pickup : MonoBehaviour
{
	#region INSTANCE_METHODS

	// Do something to an Entity that collides with this pickup
	protected abstract void Apply (Entity e);

	public void OnTriggerEnter2D(Collider2D col)
	{
		Player p = col.GetComponent<Player> ();
		if (p != null)
		{
			Apply (p.Data);
			//TODO play pickup sound?
			RegisteredObject.Destroy(gameObject);
		}
	}
	#endregion
}
