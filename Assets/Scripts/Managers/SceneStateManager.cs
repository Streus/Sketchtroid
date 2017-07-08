using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization;
using System;

[Serializable]
public class SceneStateManager : ISerializable
{
	/* Static Vars */

	// The time it takes to reset an unvisted scene
	private const float RESET_TIMER_MAX = 900f;

	private static SceneStateManager _instance;
	public static SceneStateManager instance()
	{
		if (_instance == null)
		{
			_instance = new SceneStateManager ();
			GameManager.instance.currentScene = SceneManager.GetActiveScene ().name;
		}
		return _instance;
	}

	/* Instance Vars */

	// Holds all the data save from scenes that have been visited
	private Dictionary<string, Dictionary<string, SeedBase>> scenes;

	// Tracks the time since a scene was last visited
	private Dictionary<string, float> resetTimers;

	// Scenes that the SSM should ignore
	// Ignored scenes do not save data or load data
	private HashSet<string> ignoreSet;

	/* Static Methods */


	/* Constructors */
	private SceneStateManager()
	{
		// First time instantiation
		scenes = new Dictionary<string, Dictionary<string, SeedBase>>();
		resetTimers = new Dictionary<string, float> ();
		ignoreSet = new HashSet<string> ();

		SceneManager.activeSceneChanged += activeSceneTransitioned;
	}
	public SceneStateManager(SerializationInfo info, StreamingContext context)
	{
		Type scene_type = typeof(Dictionary<string, Dictionary<string, SeedBase>>);
		Type rt_type = typeof(Dictionary<string, float>);

		scenes = (Dictionary<string, Dictionary<string, SeedBase>>)info.GetValue ("scenes", scene_type);
		resetTimers = (Dictionary<string, float>)info.GetValue ("resetTimers", rt_type);

		ignoreSet = new HashSet<string> ();
		int igSize = info.GetInt32 ("ignoreSetSize");
		for(int i = 0; i < igSize; i++)
			ignoreSet.Add((string)info.GetValue ("ignoreSet" + i, typeof(string)));

		//replace existing SSM
		SceneManager.activeSceneChanged -= _instance.activeSceneTransitioned;
		SceneManager.activeSceneChanged += activeSceneTransitioned;

		_instance = this;
	}

	/* Destructor */
	~SceneStateManager()
	{
		Debug.Log ("[SceneStateManager] Replaced SSM instance."); //DEBUG SSM destruction
	}

	/* Instance Methods */

	// Save the data for the current scene
	public void transitionTo(string nextName)
	{
		//decrement timers based on time spent in current scene
		float timeSpent = GameManager.instance.startScene();
		Dictionary<string, float> updatedTimers = new Dictionary<string, float> ();
		foreach (KeyValuePair<string, float> timer in resetTimers)
		{
			float updatedTimer = timer.Value - timeSpent;

			//remove entries if the timer duration is expended
			if (updatedTimer <= 0f)
			{
				Dictionary<string, SeedBase> sceneData;
				Dictionary<string, SeedBase> updatedSD = new Dictionary<string, SeedBase> ();
				scenes.TryGetValue (timer.Key, out sceneData);

				//check for objects that ignore reset and add them to a repo
				foreach (KeyValuePair<string, SeedBase> entry in sceneData)
				{
					if (entry.Value.ignoreReset)
						updatedSD.Add (entry.Key, entry.Value);
				}

				//if the new repo has entries, save it in place of the old one
				scenes.Remove (timer.Key);
				if (updatedSD.Count >= 0)
					scenes.Add (timer.Key, updatedSD);

				//remove this scene from the list of scenes waiting for reset
				resetTimers.Remove (timer.Key);
			}
			else
				updatedTimers.Add (timer.Key, updatedTimer);
		}
		//update all timers
		resetTimers = updatedTimers;

		SceneManager.LoadScene(nextName, LoadSceneMode.Single);

		//if the current scene is not ignored, save data from it
		if (!ignoreSet.Contains (SceneManager.GetActiveScene ().name))
		{
			Console.log.println ("[SceneStateManager] Saving " + SceneManager.GetActiveScene ().name + ".", Console.LogTag.info); //DEBUG

			//create a dictionary for the incoming data
			Dictionary<string, SeedBase> currData = new Dictionary<string, SeedBase> ();

			//add each ROs data to the dictionary
			foreach (RegisteredObject ro in RegisteredObject.getObjects())
				currData.Add (ro.rID, ro.reap ());

			//replace any old data with the new data (including resetTimer data)
			string currName = SceneManager.GetActiveScene ().name;
			scenes.Remove (currName);
			resetTimers.Remove (currName);
			scenes.Add (currName, currData);
			resetTimers.Add (currName, RESET_TIMER_MAX);
		}
		else
			Console.log.println ("[SceneStateManager] " + SceneManager.GetActiveScene ().name + " is being ignored.", Console.LogTag.info); //DEBUG

		//Do the scene transition and tell the GM what scene was entered
		SceneManager.SetActiveScene (SceneManager.GetSceneByName (nextName));
		GameManager.instance.currentScene = nextName;
	}

	// Go to a new scene without saving anything from the current scene
	public void jumpTo(string sceneName)
	{
		SceneManager.LoadScene (sceneName, LoadSceneMode.Single);
		SceneManager.SetActiveScene (SceneManager.GetSceneByName (sceneName));
		GameManager.instance.currentScene = sceneName;
	}

	// Load saved data into ROs in the new scene
	private void activeSceneTransitioned(Scene prev, Scene curr)
	{
		Console.log.println ("[SceneStateManager] Loading values for " + curr.name + ".", Console.LogTag.info); //DEBUG

		Dictionary<string, SeedBase> currData;

		//if no data is saved, exit the method
		if (!scenes.TryGetValue (curr.name, out currData))
		{
			Console.log.println ("[SceneStateManager] No data to load for " + curr.name + ".", Console.LogTag.info); //DEBUG
			return;
		}

		//spawn prefabs before starting sow cycle
		foreach (SeedBase sb in currData.Values)
		{
			if (sb.prefabPath != "")
			{
				if (RegisteredObject.recreate (sb.prefabPath, sb.registeredID, sb.parentID) != null)
					Console.log.println ("[SceneStateManager] Respawned prefab object: " + sb.registeredID + ".", Console.LogTag.info); //DEBUG
				else
					Console.log.println ("[SceneStateManager] Failed to respawn prefab object: " + sb.registeredID + ".", Console.LogTag.error); //DEBUG
			}
		}

		//iterate over the list of ROs and pass them data
		foreach (RegisteredObject ro in RegisteredObject.getObjects())
		{
			SeedBase data;
			if (currData.TryGetValue (ro.rID, out data))
				ro.sow (data);
		}
	}

	// Adds the current scene to the ignore list. Returns false if it already was in the
	//ignore list (Also clears out existing data, ifex).
	public bool ignoreCurrentScene()
	{
		string currName = SceneManager.GetActiveScene ().name;
		scenes.Remove (currName);

		bool newIgnore = ignoreSet.Add (currName);
		if(newIgnore)
			Console.log.println ("[SceneStateManager] Ignoring " + SceneManager.GetActiveScene ().name + ".", Console.LogTag.info); //DEBUG

		return newIgnore;
	}

	public void GetObjectData(SerializationInfo info, StreamingContext context)
	{
		info.AddValue ("scenes", scenes);
		info.AddValue ("resetTimers", resetTimers);

		string[] igset =  new string[ignoreSet.Count];
		ignoreSet.CopyTo (igset);
		info.AddValue ("ignoreSetSize", igset.Length);
		for(int i = 0; i < igset.Length; i++)
			info.AddValue ("ignoreSet" + i, igset[i]);
	}

	public override string ToString ()
	{
		string str = "[SceneStateManager]\n";
		str += "Currently Managing " + scenes.Count + " scenes.\n";
		foreach (string sceneName in scenes.Keys)
		{
			str += "  " + sceneName;
			if (ignoreSet.Contains (sceneName))
				str += " (ignored)";
			else
			{
				float timerValue = 0f;
				resetTimers.TryGetValue (sceneName, out timerValue);
				str += " " + timerValue.ToString ("###.00") + " seconds to reset.";
			}
			str += "\n";

			Dictionary<string, SeedBase> sceneData;
			scenes.TryGetValue (sceneName, out sceneData);
			foreach (string regObj in sceneData.Keys)
				str += "    " + regObj + "\n";
		}
		return str;
	}
}
