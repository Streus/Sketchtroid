﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

	// The states this Controller can transition in and out of
	protected BehaviorState[] states;

	/* Static Methods */


	/* Instance Methods */
	public virtual void Awake()
	{
		self = GetComponent<Entity> ();
		anim = GetComponent<Animator> ();
		physbody = GetComponent<Rigidbody2D> ();
	}

	protected bool isUpdating()
	{
		return physbody.simulated && !self.isStunned () && !self.frozen();
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

	public virtual void LateUpdate()
	{
		if (!isUpdating ())
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

	protected void faceTargetLeading(Transform target, float bulletSpeed)
	{
		Rigidbody2D body = target.GetComponent<Rigidbody2D> ();
		if (body == null)
			throw new ArgumentException ("Tried to lead a velocity-less target.");

		float stepsToCollision = Vector2.Distance (transform.position, target.position);
		facePoint ((Vector2)target.position + (body.velocity * stepsToCollision));
	}

	// Attempt to use the ability at the given index
	protected bool useAbility(int index, Vector2 targetPos, params object[] args)
	{
		try
		{
			return self.getAbility (index).use (self, targetPos, args);
		}
		#pragma warning disable 0168
		catch(IndexOutOfRangeException ioore)
		#pragma warning restore 0168
		{
			return false;
		}
	}
}
