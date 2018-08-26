using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AI;

[RequireComponent(typeof(Entity))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Controller : MonoBehaviour
{
	#region STATIC_VARS

	#endregion


	#region INSTANCE_VARS

	protected Entity self;
	public Entity Data { get { return self; } }
	protected Animator anim;
	protected Rigidbody2D physbody;

	// The initial state of this controller
	[SerializeField]
	private State activeState;
	#endregion

	#region STATIC_METHODS

	#endregion


	#region INSTANCE_METHODS
	public virtual void Awake()
	{
		self = GetComponent<Entity> ();
		anim = GetComponent<Animator> ();
		physbody = GetComponent<Rigidbody2D> ();
	}

	public void SetPause(bool pause)
	{
		self.enabled = physbody.simulated = pause;
	}

	protected bool IsUpdating()
	{
		return physbody.simulated && !self.Stunned && !self.Frozen;
	}

	public virtual void Update()
	{
		if (!IsUpdating())
			return;

		// Call currently active behavior
		if(activeState != null)
			activeState.update(this);
	}

	public virtual void FixedUpdate()
	{
		if (!IsUpdating())
			return;
		
			//TODO manage movement ?
	}

	public void SetState(State state)
	{
		activeState.exit (this);
		activeState = state;
		activeState.enter (this);
	}

	protected void FacePoint(Vector2 point)
	{
		Quaternion rot = Quaternion.LookRotation (transform.position - new Vector3(point.x, point.y, -100f), Vector3.forward);
		transform.rotation = rot;
		transform.eulerAngles = new Vector3 (0f, 0f, transform.eulerAngles.z);
	}

	protected void FaceTarget(Transform target)
	{
		if (transform != null)
			FacePoint (transform.position);
	}

	protected void FaceTargetLeading(Transform target, float bulletSpeed)
	{
		Rigidbody2D body = target.GetComponent<Rigidbody2D> ();
		if (body == null)
			throw new ArgumentException ("Tried to lead a velocity-less target.");

		float stepsToCollision = Vector2.Distance (transform.position, target.position);
		FacePoint ((Vector2)target.position + (body.velocity * stepsToCollision));
	}

	// Attempt to use the ability at the given index
	protected bool UseAbility(int index, Vector2 targetPos, params object[] args)
	{
		try
		{
			return self.GetAbility (index).Use (self, targetPos, args);
		}
		catch(NullReferenceException) { return false; }
		catch(IndexOutOfRangeException) { return false; }
	}
	#endregion
}
