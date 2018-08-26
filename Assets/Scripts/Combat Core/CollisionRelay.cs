using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollisionRelay : MonoBehaviour
{
	private Entity relayTarget;

	public void Start()
	{
		Entity target = GetComponentInParent<Entity> ();
		if (target != null && relayTarget == null)
			AddToNetwork (target);
	}

	public void AddToNetwork(Entity target)
	{
		relayTarget = target;
	}

	public void RemoveFromNetwork()
	{
		relayTarget = null;
	}

	public Entity LogCollision(Bullet bullet)
	{
		if (relayTarget == null)
			return null;

		if (!relayTarget.AddColLogEntry (bullet))
		{
			//the network has not collided with this bullet yet
			return relayTarget;
		}
		//the network has collided with this bullet, ignore this collision
		return null;
	}
}
