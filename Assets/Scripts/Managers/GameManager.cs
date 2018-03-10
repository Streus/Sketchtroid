using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System;

public enum Difficulty
{
	easy, normal, hard, expert, master
}

public class GameManager : MonoBehaviour
{
	#region STATIC_VARS
	public const int SPAWN_AT_DOOR = 0;
	public const int SPAWN_AT_SVPNT = 1;

	private static GameManager _instance;
	public static GameManager instance { get { return _instance; } }

	// The full paths to the saves and data directories
	private static string saveDirectory;
	private static string dataDirectory;
	public static string savePath { get { return saveDirectory; } }
	#endregion

	#region INSTANCE_VARS

	// The pause state of the entire game
	private bool paused;
	private bool pauseLock;

	// The system-defined name for the current game
	// (Used to name the save file)
	private string saveName;

	// The user-provided name for the current game
	// (Used in loading GUI)
	private string gameName;
	public string gameTitle { get { return gameName; } set { gameName = value; } }

	// The time that the current scene started
	private float sceneTime;

	// The total game time for the current save
	private double totalTime;

	// The Scene of the last save
	private string currScene;
	public string currentScene
	{
		get { return currScene; }
		set { prevScene = currScene; currScene = value; }
	}

	// Used for scene transitions
	private string prevScene;

	// The game difficulty level
	private Difficulty _difficulty;
	public Difficulty difficulty
	{
		get { return _difficulty; }
		set { _difficulty = value; }
	}

	// The currently active player object
	private GameObject _player;
	public GameObject player { get { return _player; } }

	// Data describing the player object when it is not instantiated
	private SeedCollection.Base playerData;

	// Used to determine whether the player is spawned at a savepoint, or via a door
	private int playerSpawnType;

	// The number of damage types unlocked by the player
	private int _damageTypeUnlocks;

	// The abilities unlocked by the player
	private int _abilities;

	#endregion

	#region STATIC_METHODS

	#endregion

	#region INSTANCE_METHODS

	public void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad (gameObject);
		}
		else
			Destroy (gameObject);

		SceneStateManager.instance ().ignoreCurrentScene ();

		paused = false;
		pauseLock = false;

		saveName = "";
		gameName = "";
		sceneTime = 0f;
		totalTime = 0.0;
		currScene = "";
		prevScene = "";
		_difficulty = Difficulty.easy;

		_player = null;
		playerData = null;
		playerSpawnType = 0;

		_damageTypeUnlocks = 0;
		unlockDT (DamageType.PHYSICAL);
	
		_abilities = 0;

		saveDirectory = Application.persistentDataPath + Path.DirectorySeparatorChar
						+ "saves" + Path.DirectorySeparatorChar;
		dataDirectory = Application.persistentDataPath + Path.DirectorySeparatorChar
						+ "data" + Path.DirectorySeparatorChar;
	}

	public void Start()
	{
		
	}

	public void Update()
	{
		//track the time spent in the current scene
		sceneTime += Time.unscaledDeltaTime;
	}

	public void setPause(bool state)
	{
		if (state != paused && !pauseLock)
		{
			paused = state;
			foreach (Rigidbody2D body in GameObject.FindObjectsOfType<Rigidbody2D>())
			{
				body.simulated = !paused;
				Controller c = body.GetComponent<Controller> ();
				if (c != null)
					c.setPause (body.simulated);
			}

			if (pauseToggled != null)
				pauseToggled (paused);
		}
	}

	public bool isPaused()
	{
		return paused;
	}

	public void setPauseLock(bool state)
	{
		pauseLock = state;
	}

	public void setGameName(string gameName)
	{
		this.gameName = gameName;
	}

	// Called by the SSM for time-keeping purposes
	public float startScene()
	{
		playerSpawnType = SPAWN_AT_DOOR;

		float timeSave = sceneTime;
		totalTime += (double)sceneTime;
		sceneTime = 0f;

		Console.log.println ("[GameManger] Time spent in scene: " + timeSave + ".", Console.LogTag.info);
		return timeSave;
	}

	// Save the currently active game
	public void saveGame()
	{
		if (saveName == "")
			saveName = DateTime.Now.Ticks.ToString ("X").ToUpper ();

		if (!Directory.Exists (saveDirectory))
			Directory.CreateDirectory (saveDirectory);
			
		FileStream file = File.Open (saveDirectory + saveName + ".save", FileMode.OpenOrCreate);

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
		save.currScene = currScene;
		save.prevScene = prevScene;
		save.difficulty = _difficulty;

		save.playerData = playerData = player.GetComponent<Entity>().reap();

		save.dtUnlocks = _damageTypeUnlocks;
		save.abilities = _abilities;

		return save;
	}

	// Save the SceneStateManager
	public void saveData()
	{
		if (!Directory.Exists (dataDirectory))
			Directory.CreateDirectory (dataDirectory);

		FileStream file = File.Open (dataDirectory + saveName + ".dat", FileMode.OpenOrCreate);

		BinaryFormatter formatter = new BinaryFormatter ();
		formatter.Serialize (file, SceneStateManager.instance());
		file.Close ();
	}

	// Load up a save from the file system
	// Returns null if the given file does not exist
	public Save loadSave(string filename)
	{
		string filepath = saveDirectory + filename + ".save";

		if (File.Exists (filepath))
		{
			FileStream file = File.Open (filepath, FileMode.Open);
			BinaryFormatter formatter = new BinaryFormatter ();
			Save save = (Save)formatter.Deserialize (file);
			file.Close ();
			return save;
		}

		return null;
	}

	// Load a SceneStateManager from the file system
	public void loadData(string filename)
	{
		string filepath = dataDirectory + filename + ".dat";

		if (File.Exists (filepath))
		{
			FileStream file = File.Open (filepath, FileMode.Open);
			BinaryFormatter formatter = new BinaryFormatter ();
			formatter.Deserialize (file);
			file.Close ();
		}
	}

	// Loads the values in a save into the GameManager for resuming a game
	public void loadGame(Save save)
	{
		saveName = save.saveName;
		gameName = save.gameName;
		sceneTime = save.sceneTime;
		totalTime = save.gameTime;
		currScene = save.currScene;
		prevScene = save.prevScene;
		_difficulty = save.difficulty;

		playerData = save.playerData;

		_damageTypeUnlocks = save.dtUnlocks;
		_abilities = save.abilities;

		loadData (saveName);

		playerSpawnType = SPAWN_AT_SVPNT;
		SceneStateManager.instance ().jumpTo (save.currScene);
	}

	// Delete all the data associated with a given save
	public bool deleteSave(string saveName)
	{
		if (File.Exists (saveDirectory + saveName + ".save"))
		{
			File.Delete (saveDirectory + saveName + ".save");
			if(File.Exists(dataDirectory + saveName + ".dat"))
				File.Delete (dataDirectory + saveName + ".dat");
			return true;
		}
		return false;
	}

	// Create a player object
	public GameObject createPlayer()
	{
		GameObject pref = Resources.Load<GameObject> ("Prefabs/Entities/Player");
		GameObject inst = Instantiate<GameObject> (pref);
		_player = inst;
		Entity e = _player.GetComponent<Entity> ();
		e.sow (playerData);
		HUDManager.instance.setSubject (e);

		switch (playerSpawnType)
		{
		case SPAWN_AT_DOOR:
			//find destination and spawn there
			SceneDoor door = SceneDoor.getDoor (prevScene);
			door.startTransitionIn (inst);
			break;
		case SPAWN_AT_SVPNT:
			CameraManager.scene_cam.setTarget(inst.transform);
			//TODO spawn at savepoint ?
			break;
		default:
			throw new ArgumentException ("Invalid spawn type code: " + playerSpawnType);
		}

		return inst;
	}

	// Save the player's data at the end of a scene
	public void savePlayer()
	{
		if(_player != null)
			playerData = _player.GetComponent<Entity> ().reap ();
	}

	// --- Unlock Management ---

	// Check if a specific damage type has been unlocked
	public bool isDTUnlocked(DamageType dt)
	{
		int check = 1 << (int)dt;
		return (_damageTypeUnlocks & check) == check;
	}

	// Unlock a given damage type
	public void unlockDT(DamageType dt)
	{
		_damageTypeUnlocks |= (1 << (int)dt);
	}

	// Check if a specific ability is unlocked
	public bool isAbilityUnlocked(Ability a)
	{
		int check = 1 << a.ID;
		return (_abilities & check) == check;
	}
	public bool isAbilityUnlocked(string name)
	{
		return isAbilityUnlocked(Ability.get (name));
	}

	// Unlock an ability
	public void unlockAbility(Ability a)
	{
		_abilities |= (1 << a.ID);
	}

	#endregion

	#region INTERNAL_TYPES

	/* Delegates and Events */
	public delegate void TogglePaused(bool state);
	public event TogglePaused pauseToggled;

	/* Inner Classes */
	[Serializable]
	public class Save : ISerializable
	{
		public string saveName;
		public string gameName;
		public float sceneTime;
		public double gameTime;
		public string currScene;
		public string prevScene;
		public Difficulty difficulty;

		public SeedCollection.Base playerData;

		public int dtUnlocks;
		public int abilities;

		public Save()
		{
			saveName = gameName = currScene = "";
			sceneTime = 0f;
			gameTime = 0.0;
			currScene = "";
			prevScene = "";
			difficulty = Difficulty.easy;

			playerData = null;

			dtUnlocks = 0;
			abilities = 0;
		}

		public Save(SerializationInfo info, StreamingContext context)
		{
			saveName = info.GetString("saveName");
			gameName = info.GetString("gameName");
			sceneTime = info.GetSingle("sceneTime");
			gameTime = info.GetDouble("gameTime");
			currScene = info.GetString("currScene");
			prevScene = info.GetString("prevScene");
			difficulty = (Difficulty)info.GetInt32("difficulty");

			playerData = (SeedCollection.Base)info.GetValue("playerData", typeof(SeedCollection.Base));

			dtUnlocks = info.GetInt32("dtUnlocks");
			abilities = info.GetInt32("abilities");
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue ("saveName", saveName);
			info.AddValue ("gameName", gameName);
			info.AddValue ("sceneTime", sceneTime);
			info.AddValue ("gameTime", gameTime);
			info.AddValue ("currScene", currScene);
			info.AddValue ("prevScene", prevScene);
			info.AddValue ("difficulty", (int)difficulty);

			info.AddValue("playerData", playerData);

			info.AddValue ("dtUnlocks", dtUnlocks);
			info.AddValue ("abilities", abilities);
		}
	}

	#endregion
}
