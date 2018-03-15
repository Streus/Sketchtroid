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
	private Image[] damageTypes;
	[SerializeField]
	private Image[] abilities;
	[SerializeField]
	private Button loadButton;
	[SerializeField]
	private Button deleteButton;

	private GameManager.Save data;

	/* Static Methods */
	public static GameSummary create(RectTransform parent, GameManager.Save data)
	{
		GameObject pref = AssetBundleUtil.loadAsset<GameObject> ("core", "GameSummary");
		GameObject inst = Instantiate<GameObject> (pref, parent, false);
		GameSummary summary = inst.GetComponent<GameSummary> ();
		summary.setName (data.gameName);
		summary.setDifficulty (data.difficulty);
		summary.setTime (data.gameTime);
		summary.setArea (data.currScene);

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
		catch(System.ArgumentOutOfRangeException aoore)
		{
			Debug.LogError (aoore.Message + " Error processing time string.");
		}
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
