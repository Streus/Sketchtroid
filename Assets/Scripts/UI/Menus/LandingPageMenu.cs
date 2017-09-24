using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingPageMenu : MonoBehaviour
{
	[SerializeField]
	private Menu nextMenu;

	public void Update()
	{
		if (Input.anyKeyDown)
		{
			if (nextMenu != null)
				MenuManager.menusys.showMenu (nextMenu);
			else
				SceneStateManager.instance().jumpTo ("test1");
		}
	}
}
