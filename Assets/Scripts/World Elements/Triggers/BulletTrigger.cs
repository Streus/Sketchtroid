using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[ExecuteInEditMode]
public class BulletTrigger : CircuitNodes.Toggle
{
	[SerializeField]
	private DamageType key;

	public void Awake()
	{
		setColor ();
	}

	#if UNITY_EDITOR
	public void Update()
	{
		if(!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
			setColor ();
	}
	#endif

	public void OnCollisionEnter2D(Collision2D col)
	{
		Bullet b = col.gameObject.GetComponent<Bullet> ();
		if (b != null && b.damageType == key)
		{
			setActive (!isActivated ());

			setColor ();
		}
	}

	private void setColor()
	{
		SpriteRenderer rend = GetComponent<SpriteRenderer> ();
		if (rend != null)
			rend.color = isActivated () ? Bullet.damageTypeToColor (key) : Color.black;
	}
}
