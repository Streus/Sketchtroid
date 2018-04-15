using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDoor : CircuitNodes.LeafNode
{
	[SerializeField]
	private float toggleSpeed = 0.1f;

	public override bool isActivated ()
	{
		bool state = base.isActivated ();
		setActive (state);
		return state;
	}

	public override void setActive (bool state)
	{
		StartCoroutine (moveDoor (state));
	}

	private IEnumerator moveDoor(bool targetState)
	{
		if (targetState)
			toggleSpeed *= -1f;

		yield return null;
		//TODO door open/close coroutine
	}
}
