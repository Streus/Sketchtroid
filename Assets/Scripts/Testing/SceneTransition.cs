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
		{
			DontDestroyOnLoad (gameObject);
			_instance = this;
		}
		else
			Destroy (gameObject);
	}

	public void transition()
	{
		Scene curr = SceneManager.GetActiveScene ();
		string nextRoom = "";

		if (curr.name.Equals("test1"))
			nextRoom = "test2";
		else if (curr.name.Equals("test2"))
			nextRoom = "test1";

		SceneStateManager.instance().transitionTo (nextRoom);
	}
}
