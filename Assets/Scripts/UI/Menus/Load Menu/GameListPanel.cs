using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameListPanel : MonoBehaviour
{
	#region INSTANCE_VARS

	[SerializeField]
	private int targetChild;
	private float targetX;
	#endregion

	#region INSTANCE_METHODS
	public void Awake()
	{
		transform.parent.GetComponent<Menu> ().changedFocus += modifyList;
		targetChild = 0;
		targetX = 0f;
	}

	public void Update()
	{
		int dC = (int)Input.mouseScrollDelta.y;

		if (dC != 0 && transform.childCount > 0)
		{
			targetChild += dC;
			calcTargetX ();
		}

		if (Mathf.Abs (targetX - transform.position.x) > 0.01f)
			transform.position = Vector2.Lerp (transform.position, new Vector2 (targetX, transform.position.y), Time.deltaTime * 5f);
		else
			transform.position = new Vector2 (targetX, transform.position.y);
	}
	public void calcTargetX()
	{
		if (targetChild < 0)
			targetChild = 0;
		else if (targetChild > transform.childCount - 1)
			targetChild = transform.childCount - 1;

		float childPosX = transform.GetChild (targetChild).position.x;
		float dX = childPosX - transform.parent.position.x;
		targetX = transform.position.x - dX;
	}

	private void modifyList(bool building)
	{
		//clear the list
		for (int i = 0; i < transform.childCount; i++)
			Destroy (transform.GetChild (i).gameObject);

		//done if leaving the menu
		if (!building)
			return;

		if (Directory.Exists (GameManager.savePath))
		{
			string[] saves = Directory.GetFiles (GameManager.savePath);
			foreach (string save in saves)
			{
				int start = save.LastIndexOf (Path.DirectorySeparatorChar) + 1;
				int end = save.LastIndexOf ('.');
				string sani_save = save.Substring (start, end - start);
				GameSummary.Create (GetComponent<RectTransform> (), GameManager.instance.loadSave (sani_save));
				Debug.Log ("Loaded " + sani_save); //DEBUG
			}
		}
		else
		{
			//TODO display special graphic for no saves
		}

		// orient the window on the first save, ifex
		targetChild = 0;
		if(transform.childCount > 0)
			calcTargetX ();
	}
	#endregion
}
