using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BasicBulletSwitch : Interactable
{
	/* Instance Vars */

	// The object this Switch will toggle
	public Activateable subject;

	// The switch's sprite
	private SpriteRenderer sprite;

	/* Instance Methods */
	public void Awake()
	{
		sprite = GetComponent<SpriteRenderer> ();
		if (activated)
			sprite.color = Color.black;
		else
			sprite.color = Bullet.damageTypeToColor(keyType);
	}

	public override void OnInteract (DamageType dt)
	{
		base.OnInteract (dt);
		subject.OnActivate (activated);
		if (activated)
			sprite.color = Color.black;
		else
			sprite.color = Bullet.damageTypeToColor(keyType);
	}
}
