using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicDoor : Activateable
{
	[SerializeField]
	[Tooltip("The object that will be toggled on and off")]
	private GameObject door;

	public override bool OnActivate(bool state = true)
	{
		door.SetActive (!door.activeSelf);
		return door.activeSelf;
	}
}
