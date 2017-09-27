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

		string[] saves = Directory.GetFiles (Application.persistentDataPath + Path.DirectorySeparatorChar + "saves");
		foreach (string save in saves)
			GameSummary.create (GetComponent<RectTransform>(), GameManager.instance.loadSave (save));
	}
}
