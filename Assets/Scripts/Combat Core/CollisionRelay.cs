using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CollisionRelay : MonoBehaviour
{
	private Entity relayTarget;

	public void addToNetwork(Entity target)
	{
		relayTarget = target;
	}

	public void removeFromNetwork()
	{
		relayTarget = null;
	}

	public Entity logCollision(Bullet bullet)
	{
		if (relayTarget == null)
			return null;

		if (!relayTarget.addColLogEntry (bullet))
		{
			//the network has not collided with this bullet yet
			return relayTarget;
		}
		//the network has collided with this bullet, ignore this collision
		return null;
	}
}
