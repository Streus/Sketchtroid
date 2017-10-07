using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class GameTypeSelector : MonoBehaviour
{
	[SerializeField]
	private Button selectInfinite;
	[SerializeField]
	private Button selectCampaign;

	private Animator animator;

	// ----- Infinite Menu Options -----

	//TODO modifications 

	[SerializeField]
	private Button infStartGame;


	// ----- Campaign Menu Options -----
	[SerializeField]
	private InputField camGameName;
	[SerializeField]
	private DifficultySelector camDifficulty;
	[SerializeField]
	private Button camStartGame;

	public void Awake()
	{
		animator = GetComponent<Animator> ();

		selectInfinite.onClick.AddListener (toggleInfMenu);
		selectCampaign.onClick.AddListener (toggleCamMenu);

		//TODO infinite mode options func

		camGameName.onValueChanged.AddListener (camChangedGameName);
		camDifficulty.valueChanged += camChangedDifficulty;
		camStartGame.onClick.AddListener (startCampaign);
	}

	private void toggleInfMenu()
	{
		animator.SetBool ("InInfMenu", !animator.GetBool ("InInfMenu"));
	}

	private void toggleCamMenu()
	{
		animator.SetBool ("InCamMenu", !animator.GetBool ("InCamMenu"));
	}

	private void camChangedGameName(string s)
	{
		GameManager.instance.setGameName (s);
	}

	private void camChangedDifficulty (Difficulty d)
	{
		GameManager.instance.difficulty = d;
	}

	private void startCampaign()
	{
		SceneStateManager.instance().jumpTo("test1"); //DEBUG change debug starting room
	}
}
