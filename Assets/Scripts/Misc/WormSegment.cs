using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormSegment : MonoBehaviour
{
	[SerializeField]
	private bool isHead;
	[SerializeField]
	private GameObject child;
	[SerializeField]
	private float trailDistance = 0.55f;

	public void Start()
	{
		Collider2D col = GetComponent<Collider2D> ();
		Collider2D childCol = child.GetComponent<Collider2D> ();
		if (col != null && childCol != null)
			Physics2D.IgnoreCollision (col, childCol);
	}

	public void LateUpdate()
	{
		if (isHead)
			updateChild ();
	}

	public void updateChild()
	{
		if (child == null)
			return;

		//rotate to face parent
		Vector2 point = (Vector2)transform.position;

		Quaternion rot = Quaternion.LookRotation (child.transform.position - new Vector3(point.x, point.y, -100f), Vector3.forward);
		child.transform.rotation = rot;
		child.transform.eulerAngles = new Vector3 (0f, 0f, transform.eulerAngles.z);

		//lock distance within trail distance
		Vector2 dPos = child.transform.position - transform.position;
		if (dPos.magnitude > trailDistance)
			child.transform.localPosition = (Vector3)(dPos.normalized * trailDistance);

		//go down the chain
		WormSegment segment = child.GetComponent<WormSegment> ();
		if (segment != null)
			segment.updateChild ();
	}
}
