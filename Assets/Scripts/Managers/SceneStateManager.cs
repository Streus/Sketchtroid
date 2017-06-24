using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization;
using System;

//TODO set up a way to update reset timers using Unity's deltaTime
[Serializable]
public class SceneStateManager : ISerializable
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
	private Dictionary<string, float> resetTimers;

	private const float RESET_TIMER_MAX = 900f;

	/* Static Methods */


	/* Constructors */
	private SceneStateManager()
	{
		// First time instantiation
		scenes = new Dictionary<string, Dictionary<uint, ISerializable>>();
		resetTimers = new Dictionary<string, float> ();
		SceneManager.activeSceneChanged += activeSceneTransitioned;
	}
	public SceneStateManager(SerializationInfo info, StreamingContext context)
	{
		Type scene_type = typeof(Dictionary<string, Dictionary<uint, ISerializable>>);
		Type rt_type = typeof(Dictionary<string, float>);
		scenes = (Dictionary<string, Dictionary<uint, ISerializable>>)info.GetValue ("scenes", scene_type);
		resetTimers = (Dictionary<string, float>)info.GetValue ("resetTimers", rt_type);
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
		resetTimers.Remove (SceneManager.GetActiveScene ().name);
		scenes.Add (SceneManager.GetActiveScene().name, currData);
		resetTimers.Add (SceneManager.GetActiveScene ().name, RESET_TIMER_MAX);

		//Do the scene transition
		SceneManager.SetActiveScene (SceneManager.GetSceneByName (nextName));
	}

	// Load saved data into ROs in the new scene
	private void activeSceneTransitioned(Scene prev, Scene curr)
	{
		Debug.Log ("Loading values for new Scene."); //DEBUG

		if (prev.name != null && !scenes.ContainsKey (prev.name))
			Debug.LogError ("Leaving a scene (" + prev.name + ") that did not save any data!"); //DEBUG

		Dictionary<uint, ISerializable> currData;

		//if no data is saved, exit the method
		if (!scenes.TryGetValue (curr.name, out currData))
			return;

		//iterate over the list of ROs and pass them data
		foreach (RegisteredObject ro in RegisteredObject.getObjects())
		{
			ISerializable data;
			if (currData.TryGetValue (ro.rID, out data))
				ro.sow (data);
			else
				Debug.LogError ("RO (" + ro.rID + ") failed to save data!"); //DEBUG
		}
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue ("scenes", scenes);
		info.AddValue ("resetTimers", resetTimers);
	}
}
