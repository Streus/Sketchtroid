using UnityEngine;
using CircuitNodes;

[RequireComponent(typeof(Collider2D))]
public class PressurePlate : Toggle
{
	public void OnTriggerEnter2D(Collider2D other)
	{
		setActive (true);
	}

	public void OnTriggerExit2D(Collider2D other)
	{
		setActive (false);
	}
}
