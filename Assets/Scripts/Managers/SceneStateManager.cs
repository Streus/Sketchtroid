using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization;
using System;

public class SceneStateManager// : ISerializable TODO make SSM serializable too
{
	/* Static Vars */
	private static SceneStateManager _instance;
	public static SceneStateManager instance()
	{
		if (_instance == null)
			_instance = new SceneStateManager ();
		return _instance;
	}

	/* Instance Vars */
	private Dictionary<string, Dictionary<uint, ISerializable>> scenes;

	/* Static Methods */


	/* Constructor */
	private SceneStateManager()
	{
		// First time instantiation
		scenes = new Dictionary<string, Dictionary<uint, ISerializable>>();
		SceneManager.activeSceneChanged += activeSceneTransitioned;
	}

	/* Destructor */
	~SceneStateManager()
	{
		SceneManager.activeSceneChanged -= activeSceneTransitioned;
	}

	/* Instance Methods */

	// Save the data for the current scene
	public void transitionTo(string nextName, LoadSceneMode mode)
	{
		SceneManager.LoadScene(nextName, mode);

		Debug.Log ("Saving current Scene."); //DEBUG

		//create a dictionary for the incoming data
		Dictionary<uint, ISerializable> currData = new Dictionary<uint, ISerializable>();

		//add each ROs data to the dictionary
		foreach (RegisteredObject ro in RegisteredObject.getObjects())
			currData.Add (ro.rID, ro.reap ());

		//replace any old data with the new data
		scenes.Remove (SceneManager.GetActiveScene().name);
		scenes.Add (SceneManager.GetActiveScene().name, currData);

		//Do the scene transition
		SceneManager.SetActiveScene (SceneManager.GetSceneByName (nextName));
	}

	// Load saved data into ROs in the new scene
	private void activeSceneTransitioned(Scene prev, Scene curr)
	{
		Debug.Log ("Loading values for new Scene."); //DEBUG

		if (prev.name != null && !scenes.ContainsKey (prev.name))
			throw new ApplicationException ("Leaving a scene (" + prev.name + ") that did not save any data!");

		Dictionary<uint, ISerializable> currData;

		//if no data is saved, exit the method
		if (!scenes.TryGetValue (curr.name, out currData))
			return;

		//iterate over the list of ROs and pass them data
		foreach (RegisteredObject ro in RegisteredObject.getObjects())
		{
			ISerializable data;
			if (!currData.TryGetValue (ro.rID, out data))
				throw new ApplicationException ("RO (" + ro.rID + ") failed to save data!");
			ro.sow (data);
		}
	}
}
