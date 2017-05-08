using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Controller : MonoBehaviour
{
	/* Static Vars */


	/* Instance Vars */
	protected Entity self;
	protected Animator anim;
	protected Rigidbody2D physbody;

	/* Static Methods */


	/* Instance Methods */
	public virtual void Awake()
	{
		self = GetComponent<Entity> ();
		anim = GetComponent<Animator> ();
		physbody = GetComponent<Rigidbody2D> ();
	}

	private bool isUpdating()
	{
		return physbody.simulated && !self.isStunned ();
	}

	public virtual void Update()
	{
		if (!isUpdating())
			return;
	}

	public virtual void FixedUpdate()
	{
		if (!isUpdating())
			return;
	}

	protected void facePoint(Vector2 point)
	{
		Quaternion rot = Quaternion.LookRotation (transform.position - (Vector3)point, Vector3.back);
		transform.rotation = rot;
		transform.eulerAngles = new Vector3 (0f, 0f, transform.eulerAngles.z);
	}

	protected void faceTarget(Transform target)
	{
		if (transform != null)
			facePoint (transform.position);
	}
}
