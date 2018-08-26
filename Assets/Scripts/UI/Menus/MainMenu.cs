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
	public void ContinueGame()
	{
		//TODO continue button functionality
		SceneStateManager.GetInstance().JumpTo("test1"); //DEBUG temp continue func
	}

	public void BeginExit()
	{
		MenuManager.menusys.DisplayYNPrompt ("Are you sure?", ExitGame, null);
	}

	private void ExitGame()
	{
		Application.Quit ();
		#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
		#endif
	}
}
