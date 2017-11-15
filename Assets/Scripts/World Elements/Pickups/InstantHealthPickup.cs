using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantHealthPickup : Pickup
{
	[SerializeField]
	private float healAmount = 20f;

	protected override void apply (Entity e)
	{
		Entity.healEntity (e, healAmount);
		//TODO heal pickup vfx and sfx
	}
}
