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
	private Dictionary<Scene, Dictionary<uint, ISerializable>> scenes;

	/* Static Methods */


	/* Constructor */
	private SceneStateManager()
	{
		// First time instantiation
		scenes = new Dictionary<Scene, Dictionary<uint, ISerializable>>();
		SceneManager.activeSceneChanged += activeSceneTransitioned;
		SceneManager.sceneLoaded += newSceneLoaded;
	}

	/* Destructor */
	~SceneStateManager()
	{
		SceneManager.activeSceneChanged -= activeSceneTransitioned;
		SceneManager.sceneLoaded -= newSceneLoaded;
	}

	/* Instance Methods */

	// Save the data for the current scene
	private void newSceneLoaded(Scene next, LoadSceneMode mode)
	{
		//create a dictionary for the incoming data
		Dictionary<uint, ISerializable> currData = new Dictionary<uint, ISerializable>();

		//add each ROs data to the dictionary
		foreach (RegisteredObject ro in RegisteredObject.getObjects())
			currData.Add (ro.rID, ro.reap ());

		//replace any old data with the new data
		scenes.Remove (SceneManager.GetActiveScene ());
		scenes.Add (SceneManager.GetActiveScene (), currData);
	}

	// Load saved data into ROs in the new scene
	private void activeSceneTransitioned(Scene prev, Scene curr)
	{
		if (!scenes.ContainsKey (prev))
			throw new ApplicationException ("Leaving a scene (" + prev.name + ") that did not save any data!");

		Dictionary<uint, ISerializable> currData;

		//if no data is saved, exit the method
		if (!scenes.TryGetValue (curr, out currData))
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
