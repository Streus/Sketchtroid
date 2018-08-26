using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class MenuManager : MonoBehaviour
{
	public static MenuManager menusys { get; private set; }

	public Menu currentMenu;
	private Menu prevMenu;
	private Menu[] menus;

	[SerializeField]
	private ErrorDisplay errDisplay;

	public void Start()
	{
		if(menusys == null)
			menusys = this;
		else
			throw new UnityException("More then one MenuManager in scene!");

		menus = FindObjectsOfType<Menu>();

		if (currentMenu != null)
			ShowMenu (currentMenu);
		else
			Debug.Log ("No default menu!");
	}

	// Retrieve a specific menu for the menu list
	public Menu GetMenu(string name)
	{
		foreach(Menu menu in menus)
			if(menu.gameObject.name == name)
				return menu;
		return null;
	}

	// Switch to menu from the current menu
	public void ShowMenu(Menu menu)
	{
		if(currentMenu != null)
			currentMenu.IsOpen = false;
		prevMenu = currentMenu;
		currentMenu = menu;
		currentMenu.IsOpen = true;
	}

	// Return to the last menu that was displayed, if there is one
	public void ReturnToPreviousMenu()
	{
		if (prevMenu != null)
			ShowMenu (prevMenu);
	}

	// Call up an error window and display error text in it.
	public void DisplayError(string errorText)
	{
		if (errDisplay != null)
		{
			errDisplay.gameObject.SetActive (true);
			errDisplay.DisplayError (errorText);
		}
	}

	// Create a yes/no prompt.
	public void DisplayYNPrompt(string desc, UnityAction accept, UnityAction decline)
	{
		Prompt.Create (GetComponent<RectTransform> (), desc, new Prompt.Option ("Yes", accept), new Prompt.Option ("No", decline));
	}
}
