using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization;
using System;

[Serializable]
public class SceneStateManager : ISerializable
{
	#region STATIC_VARS

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
	#endregion

	#region INSTANCE_VARS

	// Holds all the data save from scenes that have been visited
	private Dictionary<string, Dictionary<string, SeedCollection>> scenes;

	// Tracks the time since a scene was last visited
	private Dictionary<string, float> resetTimers;

	// Scenes that the SSM should ignore
	// Ignored scenes do not save data or load data
	private HashSet<string> ignoreSet;
	#endregion

	#region STATIC_METHODS

	#endregion

	#region INSTANCE_METHODS

	private SceneStateManager()
	{
		// First time instantiation
		scenes = new Dictionary<string, Dictionary<string, SeedCollection>>();
		resetTimers = new Dictionary<string, float> ();
		ignoreSet = new HashSet<string> ();

		SceneManager.activeSceneChanged += activeSceneTransitioned;
	}
	public SceneStateManager(SerializationInfo info, StreamingContext context)
	{
		Type scene_type = typeof(Dictionary<string, Dictionary<string, SeedCollection>>);
		Type rt_type = typeof(Dictionary<string, float>);

		scenes = (Dictionary<string, Dictionary<string, SeedCollection>>)info.GetValue ("scenes", scene_type);
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
		
	~SceneStateManager()
	{
		Debug.Log ("[SSM] Replaced instance."); //DEBUG SSM destruction
	}

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
				Dictionary<string, SeedCollection> sceneData;
				Dictionary<string, SeedCollection> updatedSD = new Dictionary<string, SeedCollection> ();
				scenes.TryGetValue (timer.Key, out sceneData);

				//check for objects that ignore reset and add them to a repo
				foreach (KeyValuePair<string, SeedCollection> entry in sceneData)
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
			Console.println ("[SSM] Saving " + SceneManager.GetActiveScene ().name + ".", Console.Tag.info, Console.nameToChannel("SSM"));

			//create a dictionary for the incoming data
			Dictionary<string, SeedCollection> currData;
			if (!scenes.TryGetValue (SceneManager.GetActiveScene ().name, out currData))
				currData = new Dictionary<string, SeedCollection> ();

			//add each ROs data to the dictionary
			foreach (RegisteredObject ro in RegisteredObject.getObjects())
			{
				if (currData.ContainsKey (ro.rID))
					currData.Remove (ro.rID);
				currData.Add (ro.rID, ro.reap ());
			}

			//replace any old data with the new data (including resetTimer data)
			string currName = SceneManager.GetActiveScene ().name;
			scenes.Remove (currName);
			resetTimers.Remove (currName);
			scenes.Add (currName, currData);
			resetTimers.Add (currName, RESET_TIMER_MAX);
		}
		else
			Console.println ("[SSM] " + SceneManager.GetActiveScene ().name + " is being ignored.", Console.Tag.info, Console.nameToChannel("SSM"));

		//do the scene transition and tell the GM what scene was entered
		SceneManager.SetActiveScene (SceneManager.GetSceneByName (nextName));
		GameManager.instance.currentScene = nextName;
		GameManager.instance.savePlayer ();
	}

	// Go to a new scene without saving anything from the current scene
	public void jumpTo(string sceneName)
	{
		SceneManager.LoadScene (sceneName, LoadSceneMode.Single);
		SceneManager.SetActiveScene (SceneManager.GetSceneByName (sceneName));
		GameManager.instance.currentScene = sceneName;
		GameManager.instance.savePlayer ();
	}

	// Load saved data into ROs in the new scene
	private void activeSceneTransitioned(Scene prev, Scene curr)
	{
		//create a player object from saved data
		if(curr.name != "main") //TODO see if there's a better way to do this
			GameManager.instance.createPlayer();

		Console.println ("[SSM] Loading values for " + curr.name + ".", Console.Tag.info, Console.nameToChannel("SSM"));

		Dictionary<string, SeedCollection> currData;

		//if no data is saved, exit the method
		if (!scenes.TryGetValue (curr.name, out currData))
		{
			Console.println ("[SSM] No data to load for " + curr.name + ".", Console.Tag.info, Console.nameToChannel("SSM"));
			return;
		}

		//spawn prefabs before starting sow cycle
		foreach (SeedCollection sb in currData.Values)
		{
			if (sb.prefabPath != "")
			{
				if (RegisteredObject.recreate (sb.prefabPath, sb.registeredID, sb.parentID) != null)
					Console.println ("[SSM] Respawned prefab object: " + sb.registeredID + ".", Console.Tag.info, Console.nameToChannel("SSM"));
				else
					Console.println ("[SSM] Failed to respawn prefab object: " + sb.registeredID + ".", Console.Tag.error, Console.nameToChannel("SSM"));
			}
		}

		//iterate over the list of ROs and pass them data
		foreach (RegisteredObject ro in RegisteredObject.getObjects())
		{
			SeedCollection data;
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
			Console.println ("[SSM] Ignoring " + SceneManager.GetActiveScene ().name + ".", Console.Tag.warning, Console.nameToChannel("SSM"));

		return newIgnore;
	}

	// Called by RegisteredObjects when their client components are destroyed in gameplay.
	// Places the passed seed into the current scene's dictionary
	public void store(string ID, SeedCollection seed)
	{
		//get the data for the current scene. if none exists, create a container
		Dictionary<string, SeedCollection> currData;
		if (!scenes.TryGetValue (SceneManager.GetActiveScene ().name, out currData))
		{
			currData = new Dictionary<string, SeedCollection> ();
			scenes.Add (SceneManager.GetActiveScene ().name, currData);
		}

		//add the entry to the current scene dictionary
		if (currData.ContainsKey (ID))
			currData.Remove (ID);
		currData.Add (ID, seed);

		Console.println ("[SSM] Created destruction entry for " + ID + ".", Console.Tag.info, Console.nameToChannel("SSM"));
	}

	// For serialization
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
		
	// Resolve the current SSM instance to a string
	public override string ToString ()
	{
		string str = "[SSM]\n";
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

			Dictionary<string, SeedCollection> sceneData;
			scenes.TryGetValue (sceneName, out sceneData);
			foreach (string regObj in sceneData.Keys)
				str += "    " + regObj + "\n";
		}
		return str;
	}
	#endregion
}
