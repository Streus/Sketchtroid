using UnityEngine;
using CircuitNodes;

[RequireComponent(typeof(Collider2D))]
public class PressurePlate : Toggle
{
	public void OnTriggerEnter2D(Collider2D other)
	{
		if(other.GetComponent<Bullet>() == null)
			setActive (true);
	}

	public void OnTriggerExit2D(Collider2D other)
	{
		if(other.GetComponent<Bullet>() == null)
			setActive (false);
	}
}
