using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenu : MonoBehaviour
{
	/* Instance Vars */

	/* Instance Methods */
	public void continueGame()
	{
		//TODO continue button functionality
		SceneStateManager.getInstance().jumpTo("test1"); //DEBUG temp continue func
	}

	public void beginExit()
	{
		MenuManager.menusys.displayYNPrompt ("Are you sure?", exitGame, null);
	}

	private void exitGame()
	{
		Application.Quit ();
		#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
		#endif
	}
}
