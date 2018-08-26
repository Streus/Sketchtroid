using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameSummary : MonoBehaviour
{
	#region INSTANCE_VARS
	
	[SerializeField]
	private Text gameName;
	[SerializeField]
	private Image difficulty;
	[SerializeField]
	private Sprite[] difficultyInsignias;
	[SerializeField]
	private Text time;
	[SerializeField]
	private Text area;
	[SerializeField]
	private Image[] damageTypes;
	[SerializeField]
	private Image[] abilities;
	[SerializeField]
	private Button loadButton;
	[SerializeField]
	private Button deleteButton;
	
	private GameManager.Save data;
	#endregion

	#region STATIC_METHODS

	public static GameSummary Create(RectTransform parent, GameManager.Save data)
	{
		GameObject pref = ABU.LoadAsset<GameObject> ("core", "GameSummary");
		GameObject inst = Instantiate<GameObject> (pref, parent, false);
		GameSummary summary = inst.GetComponent<GameSummary> ();
		summary.SetName (data.gameName);
		summary.SetDifficulty (data.difficulty);
		summary.SetTime (data.gameTime);
		summary.SetArea (data.currScene);

		for (int i = 0; i < summary.damageTypes.Length; i++)
		{
			if ((data.dtUnlocks & (1 << (i + 1))) == 0)
				summary.damageTypes [i].color = Color.clear;
		}

		for (int i = 0; i < summary.abilities.Length; i++)
		{
			if ((data.abilities & (1 << i)) == 0)
				summary.abilities [i].color = Color.clear;
		}

		summary.data = data;

		return summary;
	}
	#endregion

	#region INSTANCE_VARS

	public void Start()
	{
		loadButton.onClick.AddListener (StartGame);
		deleteButton.onClick.AddListener (DeleteSave);
	}

	public void SetName(string name)
	{
		gameName.text = name;
	}

	public void SetDifficulty(Difficulty difficulty)
	{
		this.difficulty.color = Bullet.DamageTypeToColor ((DamageType) ((int)difficulty + 2));
		this.difficulty.sprite = difficultyInsignias [(int)difficulty];
	}

	public void SetTime(double time)
	{
		System.TimeSpan f_time = System.TimeSpan.FromSeconds (time);
		string timeString = f_time.ToString ();
		try
		{
			timeString = timeString.Substring(0, timeString.LastIndexOf ('.'));
		}
		catch(System.ArgumentOutOfRangeException aoore)
		{
			Debug.LogError (aoore.Message + " Error processing time string.");
		}
		this.time.text = "Game Time: " + timeString;
	}

	public void SetArea(string areaName)
	{
		this.area.text = "Location: " + areaName;
	}

	private void StartGame()
	{
		GameManager.instance.loadGame (data);
	}

	private void DeleteSave()
	{
		GameManager.instance.deleteSave (data.saveName);
		transform.parent.GetComponent<GameListPanel> ().calcTargetX ();
		Destroy (gameObject);
	}
	#endregion
}
