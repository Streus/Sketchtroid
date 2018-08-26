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

		selectInfinite.onClick.AddListener (ToggleInfMenu);
		selectCampaign.onClick.AddListener (ToggleCamMenu);

		//TODO infinite mode options func

		camGameName.onValueChanged.AddListener (CamChangedGameName);
		camDifficulty.valueChanged += CamChangedDifficulty;
		camStartGame.onClick.AddListener (StartCampaign);
	}

	private void ToggleInfMenu()
	{
		animator.SetBool ("InInfMenu", !animator.GetBool ("InInfMenu"));
	}

	private void ToggleCamMenu()
	{
		animator.SetBool ("InCamMenu", !animator.GetBool ("InCamMenu"));
	}

	private void CamChangedGameName(string s)
	{
		GameManager.instance.setGameName (s);
	}

	private void CamChangedDifficulty (Difficulty d)
	{
		GameManager.instance.difficulty = d;
	}

	private void StartCampaign()
	{
		SceneStateManager.GetInstance().JumpTo("test1"); //DEBUG change debug starting room
	}
}
