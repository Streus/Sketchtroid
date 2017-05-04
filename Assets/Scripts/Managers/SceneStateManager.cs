using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStateManager
{
	/* Static Vars */
	private static SceneStateManager _instance;
	public static SceneStateManager instance
	{
		get
		{
			if (_instance == null)
				_instance = new SceneStateManager ();
			return _instance;
		}
	}

	/* Instance Vars */
	private Dictionary<Scene, SceneState> scenes;

	/* Static Methods */


	/* Constructor */
	private SceneStateManager()
	{
		// First time instantiation
		scenes = new Dictionary<Scene, SceneState>();
		SceneManager.activeSceneChanged += activeSceneTransitioned;
	}

	/* Instance Methods */
	private void activeSceneTransitioned(Scene prev, Scene curr)
	{
		// Save registered Entities from the old Scene
		//TODO

		// Load saved data into registered Entities if it exists
		//TODO
	}

	// Private inner class that represents the saved data from a scene
	private struct SceneState
	{
		//TODO list of entities, controllers, interactable objects, etc
	}
}
