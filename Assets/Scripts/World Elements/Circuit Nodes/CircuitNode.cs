using UnityEngine;

/// <summary>
/// For "active" world elements such as switches, doors, and intermediary nodes, like
/// logic gates, clocks, delays, etc.
/// </summary>
[DisallowMultipleComponent]
public abstract class CircuitNode : MonoBehaviour
{
	// Returns the "powered" state of the node
	public abstract bool isActivated();

	// Sets the "powered" state of the node
	public abstract void setActive(bool state);

	public virtual void OnDrawGizmos()
	{
		#if UNITY_EDITOR
		UnityEditor.Handles.color = isActivated() ? Color.green : Color.gray;
		UnityEditor.Handles.DrawWireDisc(transform.position, Vector3.forward, 1f);
		#endif
	}
}
