using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameSummary : MonoBehaviour
{
	/* Static Vars */


	/* Instance Vars */
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
	private Button loadButton;
	[SerializeField]
	private Button deleteButton;

	private GameManager.Save data;

	//TODO ability/player appearance display

	/* Static Methods */
	public static GameSummary create(RectTransform parent, GameManager.Save data)
	{
		GameObject pref = Resources.Load<GameObject> ("Prefabs/UI/GameSummary");
		GameObject inst = Instantiate<GameObject> (pref, parent, false);
		GameSummary summary = inst.GetComponent<GameSummary> ();
		summary.setName (data.gameName);
		summary.setDifficulty (data.difficulty);
		summary.setTime (data.gameTime);
		summary.setArea (data.currScene);
		summary.data = data;

		return summary;
	}

	/* Instance Methods */
	public void Start()
	{
		loadButton.onClick.AddListener (startGame);
		deleteButton.onClick.AddListener (deleteSave);
	}

	public void setName(string name)
	{
		gameName.text = name;
	}

	//TODO create sprites for each difficulty
	public void setDifficulty(Difficulty difficulty)
	{
		this.difficulty.color = Bullet.damageTypeToColor ((DamageType) ((int)difficulty + 2));
		this.difficulty.sprite = difficultyInsignias [(int)difficulty];
	}

	public void setTime(double time)
	{
		System.TimeSpan f_time = System.TimeSpan.FromSeconds (time);
		string timeString = f_time.ToString ();
		try
		{
			timeString = timeString.Substring(0, timeString.LastIndexOf ('.'));
		}
		catch(System.ArgumentOutOfRangeException aoore) { }
		this.time.text = "Game Time: " + timeString;
	}

	public void setArea(string areaName)
	{
		this.area.text = "Location: " + areaName;
	}

	private void startGame()
	{
		GameManager.instance.loadGame (data);
	}

	private void deleteSave()
	{
		GameManager.instance.deleteSave (data.saveName);
		transform.parent.GetComponent<GameListPanel> ().calcTargetX ();
		Destroy (gameObject);
	}
}
