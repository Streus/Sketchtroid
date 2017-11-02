using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WormSegment : MonoBehaviour
{
	[SerializeField]
	private GameObject parent;
	[SerializeField]
	private float trailDistance = -0.55f;

	public void Update()
	{
		Vector2 point = (Vector2)parent.transform.position;

		Quaternion rot = Quaternion.LookRotation (transform.position - new Vector3(point.x, point.y, -100f), Vector3.forward);
		transform.rotation = rot;
		transform.eulerAngles = new Vector3 (0f, 0f, transform.eulerAngles.z);

		Vector3 targetPos = parent.transform.position + (parent.transform.up * trailDistance);
		transform.position = Vector3.LerpUnclamped (transform.position, targetPos, Time.deltaTime * 25f);
	}
}
