using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealOverTimePickup : Pickup
{
	[SerializeField]
	private float healAmount = 30f;
	[SerializeField]
	private float tickRate = 1f;
	[SerializeField]
	private float duration = 3f;

	protected override void apply (Entity e)
	{
		e.addStatus (new Status (
			"Heal",
			"Gaining health over time",
			"",
			Status.DecayType.communal,
			1,
			duration,
			new StatusComponents.HealOverTime (healAmount / duration * tickRate, tickRate)));
		//TODO h o t pickup vfx and sfx
	}
}
