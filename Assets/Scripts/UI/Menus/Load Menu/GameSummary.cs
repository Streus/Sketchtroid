using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSummary : MonoBehaviour
{
	/* Static Vars */


	/* Instance Vars */
	[SerializeField]
	private Text gameName;
	[SerializeField]
	private Image difficulty;
	[SerializeField]
	private Text time;
	[SerializeField]
	private Text area;

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

	public void setName(string name)
	{
		gameName.text = name;
	}

	//TODO create sprites for each difficulty
	public void setDifficulty(Difficulty difficulty)
	{
		switch (difficulty)
		{
		case Difficulty.easy:
			this.difficulty.color = Color.cyan;
			break;
		case Difficulty.normal:
			this.difficulty.color = Color.green;
			break;
		case Difficulty.hard:
			this.difficulty.color = Color.yellow;
			break;
		case Difficulty.expert:
			this.difficulty.color = Color.red;
			break;
		case Difficulty.master:
			this.difficulty.color = Color.magenta;
			break;
		}
	}

	public void setTime(double time)
	{
		System.DateTime f_time = System.DateTime.FromOADate (time);
		this.time.text = "Game Time: " + f_time.ToString ();
	}

	public void setArea(string areaName)
	{
		this.area.text = "Location: " + areaName;
	}

	public void startGame()
	{
		GameManager.instance.loadGame (data);
	}
}
