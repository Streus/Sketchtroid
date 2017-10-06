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

	public void spawnRegisteredPrefabs(int amount)
	{
		for (int i = 0; i < amount; i++)
		{
			RegisteredObject.create (
				"TestEntityPrefab", 
				(Vector3)Random.insideUnitCircle, 
				Quaternion.identity);
		}
	}

	public void spawnRegisteredChildPrefab(RegisteredObject obj)
	{
		RegisteredObject.create ("TestEntityPrefab",
			Vector3.zero,
			Quaternion.identity,
			obj.transform);
	}

	public void performSave(string gameName)
	{
		GameManager.instance.setGameName ("testSave");
		GameManager.instance.gameTitle = gameName;
		GameManager.instance.saveGame ();
		GameManager.instance.saveData ();
	}

	public void performLoad(string saveName)
	{
		GameManager.Save save = GameManager.instance.loadSave (saveName);
		GameManager.instance.loadGame (save);
		GameManager.instance.loadData (saveName);
	}

	public void debugSSM()
	{
		Debug.Log (SceneStateManager.instance ().ToString ());
	}
}
