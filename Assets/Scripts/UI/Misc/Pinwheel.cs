using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Pinwheel : MonoBehaviour
{
	/* Instance Vars */
	[SerializeField]
	private float initialRotation = 90f;
	[SerializeField]
	private float distanceToCenter = 1f;

	/* Instance Methods */
	public void Update()
	{
		applyLayout ();
	}

	private void applyLayout()
	{
		//in the case of one child, do not apply offset or rotation
		if (transform.childCount == 1)
		{
			transform.GetChild (0).localPosition = Vector2.zero;
			return;
		}

		//two or more children
		for (int i = 0; i < transform.childCount; i++)
		{
			//determine rotation
			float theta = (i * (360f/transform.childCount)) + initialRotation;
			theta *= Mathf.Deg2Rad;
			Transform petal = transform.GetChild (i);

			//set distance from center
			petal.localPosition = new Vector2 (distanceToCenter, 0f);

			//rotate that bitch
			petal.localPosition = new Vector2 (
				(petal.localPosition.x * Mathf.Cos (theta)) - (petal.localPosition.y * Mathf.Sin (theta)),
				(petal.localPosition.x * Mathf.Sin (theta)) + (petal.localPosition.y * Mathf.Cos (theta)));
		}
	}
}
