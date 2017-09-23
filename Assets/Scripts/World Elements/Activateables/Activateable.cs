using UnityEngine;

public abstract class Activateable : MonoBehaviour
{
	// Is passed the state to set, returns the state that was set
	public abstract bool OnActivate (bool state = true);
}
