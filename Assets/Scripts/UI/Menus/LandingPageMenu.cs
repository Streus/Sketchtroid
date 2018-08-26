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
				MenuManager.menusys.ShowMenu (nextMenu);
			else
				SceneStateManager.GetInstance().JumpTo ("test1");

			active = false;
		}
	}
}
