using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingPageMenu : MonoBehaviour
{
	[SerializeField]
	private Menu nextMenu;
	private bool active = true;

	public void Update()
	{
		if (Input.anyKeyDown && active)
		{
			if (nextMenu != null)
				MenuManager.menusys.showMenu (nextMenu);
			else
				SceneStateManager.getInstance().jumpTo ("test1");

			active = false;
		}
	}
}
