using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[ExecuteInEditMode]
public class BasicBulletSwitch : Interactable
{
	/* Instance Vars */

	// The object this Switch will toggle
	public Activateable[] subjects;

	// Only allow this Switch to be toggled once
	[SerializeField]
	private bool lockState = false;

	// The switch's sprite
	private SpriteRenderer sprite;

	/* Instance Methods */
	public void Awake()
	{
		sprite = GetComponent<SpriteRenderer> ();
		setColor ();
	}

	#if UNITY_EDITOR
	public void Update()
	{
		setColor ();
	}
	#endif

	public override void doInteract ()
	{
		if(subjects != null)
			foreach(Activateable a in subjects)
				a.OnActivate (activated);
		setColor ();
		if (lockState)
			interactable = false;
	}

	// Remove a lock on the interactablity of this switch
	public void unlockState()
	{
		lockState = false;
		interactable = true;
	}

	private void setColor()
	{
		if (activated)
			sprite.color = Color.black;
		else
			sprite.color = Bullet.damageTypeToColor(keyType);
	}
}
