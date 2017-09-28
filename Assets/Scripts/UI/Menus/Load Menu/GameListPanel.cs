using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameListPanel : MonoBehaviour
{
	/* Static Vars */


	/* Instance Vars */


	/* Static Methods */


	/* Instance Methods */
	public void Awake()
	{
		transform.parent.GetComponent<Menu> ().changedFocus += modifyList;
	}

	private void modifyList(bool building)
	{
		//clear the list
		for (int i = 0; i < transform.childCount; i++)
			Destroy (transform.GetChild (i).gameObject);

		//done if leaving the menu
		if (!building)
				return;

		string[] saves = Directory.GetFiles (GameManager.savePath);
		foreach (string save in saves)
		{
			int start = save.LastIndexOf (Path.DirectorySeparatorChar) + 1;
			int end = save.LastIndexOf ('.');
			string sani_save = save.Substring (start, end - start);
			Debug.Log ("Loaded " + sani_save); //DEBUG
			GameSummary.create (GetComponent<RectTransform> (), GameManager.instance.loadSave (sani_save));
		}
	}
}
