using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantHealthPickup : Pickup
{
	[SerializeField]
	private float healAmount = 20f;

	protected override void Apply (Entity e)
	{
		Entity.HealEntity (e, healAmount);
		//TODO heal pickup vfx and sfx
	}
}
