using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour
{
	public static MenuManager menusys;

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
			showMenu (currentMenu);
		else
			Debug.Log ("No default menu!");
	}

	// Retrieve a specific menu for the menu list
	public Menu getMenu(string name)
	{
		foreach(Menu menu in menus)
			if(menu.gameObject.name == name)
				return menu;
		return null;
	}

	// Switch to menu from the current menu
	public void showMenu(Menu menu)
	{
		if(currentMenu != null)
			currentMenu.IsOpen = false;
		prevMenu = currentMenu;
		currentMenu = menu;
		currentMenu.IsOpen = true;
	}

	// Return to the last menu that was displayed, if there is one
	public void returnToPreviousMenu()
	{
		if (prevMenu != null)
			showMenu (prevMenu);
	}

	// Call up an error window and display error text in it.
	public void displayError(string errorText)
	{
		if (errDisplay != null)
		{
			errDisplay.gameObject.SetActive (true);
			errDisplay.displayError (errorText);
		}
	}
}
