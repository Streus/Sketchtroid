using UnityEngine;
using CircuitNodes;

[RequireComponent(typeof(Collider2D))]
public class PressurePlate : Toggle
{
	private int occupants;

	public void Awake()
	{

		occupants = 0;
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (other.GetComponent<Bullet> () == null)
		{
			occupants++;
			setActive (true);
		}
	}

	public void OnTriggerExit2D(Collider2D other)
	{
		if (other.GetComponent<Bullet> () == null)
		{
			if (--occupants <= 0)
			{
				occupants = 0;
				setActive (false);
			}
		}
	}
}
