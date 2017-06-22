using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
	private static SceneTransition _instance;
	public void Awake()
	{
		if (_instance == null)
			DontDestroyOnLoad (gameObject);
		else
			Destroy (gameObject);

		//create a SSM if it doesn't exist
		SceneStateManager.instance();
	}

	public void transition()
	{
		Scene curr = SceneManager.GetActiveScene ();
		string nextRoom = "";

		if (curr.Equals (SceneManager.GetSceneByName ("test1")))
			nextRoom = "test2";
		else if (curr.Equals (SceneManager.GetSceneByName ("test2")))
			nextRoom = "test1";
		
		SceneManager.LoadScene (nextRoom, LoadSceneMode.Single);
		SceneManager.SetActiveScene (SceneManager.GetSceneByName(nextRoom));
	}
}
