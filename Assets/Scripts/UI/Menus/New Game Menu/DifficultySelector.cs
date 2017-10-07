using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultySelector : MonoBehaviour
{
	/* Instance Vars */
	[SerializeField]
	private int targetChild;
	private float targetY;

	/* Static Methods */


	/* Instance Methods */
	public void Awake()
	{
		calcTargetY ();

		for (int i = 0; i < transform.childCount; i++)
			transform.GetChild (i).GetComponent<Image> ().color = Bullet.damageTypeToColor ((DamageType)(i + 2));
	}

	public void Update()
	{
		int dC = -(int)Input.mouseScrollDelta.y;

		if (dC != 0 && transform.childCount > 0)
		{
			targetChild += dC;
			calcTargetY ();

			if (valueChanged != null)
				valueChanged ((Difficulty)targetChild);
		}

		if (Mathf.Abs (targetY - transform.position.y) > 0.01f)
			transform.position = Vector2.Lerp (transform.position, new Vector2 (transform.position.x, targetY + 5), Time.deltaTime * 5f);
		else
			transform.position = new Vector2 (transform.position.x, targetY + 5);
	}
	public void calcTargetY()
	{
		if (targetChild < 0)
			targetChild = 0;
		else if (targetChild > transform.childCount - 1)
			targetChild = transform.childCount - 1;

		float childPosY = transform.GetChild (targetChild).position.y;
		float dY = childPosY - transform.parent.position.y;
		targetY = transform.position.y - dY;
	}

	public delegate void DifficultyChange(Difficulty d);
	public event DifficultyChange valueChanged;
}
