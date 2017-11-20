using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(RegisteredObject))]
public class AbilityPickup : Pickup
{
	[SerializeField]
	private string ability;
	private Ability abilData;

	[SerializeField]
	private SpriteRenderer icon;


	public void Update()
	{
		#if UNITY_EDITOR
		abilData = Ability.get (ability);
		if (abilData != null)
		{
			icon.sprite = abilData.icon;
		}
		#endif
	}


	protected override void apply (Entity e)
	{
		GameManager.instance.unlockAbility (abilData);
		RegisteredObject.destroy (gameObject);
	}
}
