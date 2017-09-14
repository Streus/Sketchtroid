using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	/* Static Vars */
	private static GameManager _instance;
	public static GameManager instance { get { return _instance; } }

	/* Instance Vars */

	// The system-defined name for the current game
	// (Used to name the save file)
	private string saveName;

	// The user-provided name for the current game
	// (Used in loading GUI)
	private string gameName;

	// The time that the current scene started
	private float sceneTime;

	// The total game time for the current save
	private double totalTime;

	// The Scene of the last save
	private string lastScene;

	// Used for scene transitions
	private string prevScene;
	private string destinationName;

	// The game difficulty level
	private Difficulty _difficulty;
	public Difficulty difficulty { get { return _difficulty; } }

	/* Static Methods */


	/* Instance Methods */
	public void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad (gameObject);
		}
		else
			Destroy (gameObject);

		//SceneStateManager.instance ().ignoreCurrentScene (); //TODO uncomment this eventually

		saveName = "";
		gameName = "";
		sceneTime = 0f;
		totalTime = 0.0;
	}

	public void Update()
	{
		//track the time spent in the current scene
		sceneTime += Time.unscaledDeltaTime;
	}

	//Getters/Setters
	public void setSaveName(string saveName)
	{
		this.saveName = saveName;
	}

	public string currentScene { get { return lastScene; } set { prevScene = lastScene; lastScene = value; } }

	public string gameTitle { get { return gameName; } set { gameName = value; } }

	// Called by the SSM for time-keeping purposes
	public float startScene()
	{
		float timeSave = sceneTime;
		totalTime += (double)sceneTime;
		sceneTime = 0f;

		Console.log.println ("[GameManger] Time spent in scene: " + timeSave + ".", Console.LogTag.info);
		return timeSave;
	}

	// Save the currently active game
	public void saveGame()
	{
		FileStream file = File.Open (Application.persistentDataPath + Path.DirectorySeparatorChar
			+ saveName + ".save", FileMode.OpenOrCreate);

		BinaryFormatter formatter = new BinaryFormatter ();
		formatter.Serialize (file, fillSave ());
		file.Close ();

		saveData ();
	}
	private Save fillSave()
	{
		Save save = new Save ();

		save.gameName = gameName;
		save.saveName = saveName;
		save.sceneTime = sceneTime;
		save.gameTime = totalTime;
		save.lastScene = lastScene;
		save.difficulty = _difficulty;

		return save;
	}

	// Save the SceneStateManager
	public void saveData()
	{
		FileStream file = File.Open (Application.persistentDataPath + Path.DirectorySeparatorChar
			+ saveName + ".dat", FileMode.OpenOrCreate);

		BinaryFormatter formatter = new BinaryFormatter ();
		formatter.Serialize (file, SceneStateManager.instance());
		file.Close ();
	}

	// Load up a save from the file system
	// Returns null if the given file does not exist
	public Save loadSave(string filename)
	{
		string filepath = Application.persistentDataPath + Path.DirectorySeparatorChar + filename + ".save";

		if (File.Exists (filepath))
		{
			FileStream file = File.Open (filepath, FileMode.Open);
			BinaryFormatter formatter = new BinaryFormatter ();
			return (Save)formatter.Deserialize (file);
		}

		return null;
	}

	// Load a SceneStateManager from the file system
	public void loadData(string filename)
	{
		string filepath = Application.persistentDataPath + Path.DirectorySeparatorChar + filename + ".dat";

		if (File.Exists (filepath))
		{
			FileStream file = File.Open (filepath, FileMode.Open);
			BinaryFormatter formatter = new BinaryFormatter ();
			formatter.Deserialize (file);
		}
	}

	// Loads the values in a save into the GameManager for resuming a game
	public void loadGame(Save save)
	{
		saveName = save.saveName;
		gameName = save.gameName;
		sceneTime = save.sceneTime;
		totalTime = save.gameTime;
		lastScene = save.lastScene;
		_difficulty = save.difficulty;

		loadData (gameName);

		SceneStateManager.instance ().jumpTo (save.lastScene);
	}

	public GameObject createPlayer(SeedBase data, Vector2 savePosition)
	{
		GameObject player = createPlayer (data);
		player.transform.position = (Vector3)savePosition;
	}
	public GameObject createPlayer(SeedBase data)
	{
		//find destination
		SceneDoor door = SceneDoor.getDoor(prevScene);

		GameObject pref = Resources.Load<GameObject> ("Prefabs/Entities/Player");
		GameObject inst = Instantiate<GameObject> (pref);

		door.startTransitionIn (inst);
	}

	/* Delegates and Events */


	/* Inner Classes */
	[Serializable]
	public class Save : ISerializable
	{
		public string saveName;
		public string gameName;
		public float sceneTime;
		public double gameTime;
		public string lastScene;
		public Difficulty difficulty;

		public Save()
		{
			saveName = gameName = lastScene = "";
			sceneTime = 0f;
			gameTime = 0.0;
			difficulty = Difficulty.easy;
		}

		public Save(SerializationInfo info, StreamingContext context)
		{
			saveName = info.GetString("saveName");
			gameName = info.GetString("gameName");
			sceneTime = info.GetSingle("sceneTime");
			gameTime = info.GetDouble("gameTime");
			lastScene = info.GetString("lastScene");
			difficulty = (Difficulty)info.GetInt32("difficulty");
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue ("saveName", saveName);
			info.AddValue ("gameName", gameName);
			info.AddValue ("sceneTime", sceneTime);
			info.AddValue ("gameTime", gameTime);
			info.AddValue ("lastScene", lastScene);
			info.AddValue ("difficulty", (int)difficulty);
		}
	}
}

public enum Difficulty
{
	easy, normal, hard, expert, master
}
