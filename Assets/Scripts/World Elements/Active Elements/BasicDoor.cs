using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDoor : CircuitNodes.LeafNode
{
	[SerializeField]
	private float toggleSpeed = 0.1f;

	public override bool IsActivated ()
	{
		bool state = base.IsActivated ();
		SetActive (state);
		return state;
	}

	public override void SetActive (bool state)
	{
		StartCoroutine (MoveDoor (state));
	}

	private IEnumerator MoveDoor(bool targetState)
	{
		if (targetState)
			toggleSpeed *= -1f;

		yield return null;
		//TODO door open/close coroutine
	}
}
