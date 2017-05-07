using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(Animator))]
public class Controller : MonoBehaviour
{
	/* Static Vars */


	/* Instance Vars */
	private Entity self;
	private Animator anim;

	/* Static Methods */


	/* Instance Methods */
	public void Awake()
	{
		self = GetComponent<Entity> ();
		anim = GetComponent<Animator> ();
	}
}
