using System.Collections;
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
	private BehaviorState activeState;

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

	public void Update()
	{
		if (!isUpdating())
			return;

		// Call currently active behavior
		if(activeState.update != null)
			activeState.update ();
	}

	public void FixedUpdate()
	{
		if (!isUpdating())
			return;

		// Call currently active behavior
		if(activeState.fixedUpdate != null)
			activeState.fixedUpdate ();
	}

	public void LateUpdate()
	{
		if (!isUpdating ())
			return;

		// Call currently active behavior
		if(activeState.lateUpdate != null)
			activeState.lateUpdate ();
	}

	protected void setState(BehaviorState state)
	{
		activeState = state;
	}

	protected void facePoint(Vector2 point)
	{
		Quaternion rot = Quaternion.LookRotation (transform.position - new Vector3(point.x, point.y, -100f), Vector3.forward);
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
			Console.log.println (ioore.Message, Console.LogTag.error);
			return false;
		}
	}

	/* Delegates */

	// Used for Update, FixedUpdate, and LateUpdate
	protected delegate void UpdateBehavior();

	/* Private Inner Class */
	protected struct BehaviorState
	{
		public string name;

		public UpdateBehavior update;
		public UpdateBehavior fixedUpdate;
		public UpdateBehavior lateUpdate;

		public BehaviorState(string name, UpdateBehavior update, UpdateBehavior fixedUpdate, UpdateBehavior lateUpdate)
		{
			this.name = name;

			this.update = update;
			this.fixedUpdate = fixedUpdate;
			this.lateUpdate = lateUpdate;
		}

		public override bool Equals (object obj)
		{
			return ((BehaviorState)obj).name == name;
		}
		public override int GetHashCode ()
		{
			return name.GetHashCode ();
		}
	}
}
