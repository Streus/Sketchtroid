﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class AbilitySelector : MonoBehaviour
{
	private const int ABIL_INDEX_MAX = 3;

	/* Static Vars */

	/* Instance Vars */
	[SerializeField]
	private Image abilIcon;
	[SerializeField]
	private Toggle toggle;
	[SerializeField]
	private int abilityIndex;
	[SerializeField]
	private string abilityName;
	private Ability ability;

	/* Static Methods */
	public static AbilitySelector create(Transform parent, string abilityName, int abilityIndex, ToggleGroup tg)
	{
		GameObject pref = Resources.Load<GameObject> ("Prefabs/UI/HUD/AbilityToggle");
		GameObject inst = Instantiate<GameObject> (pref, parent, false);
		AbilitySelector aSel = inst.GetComponent<AbilitySelector> ();
		aSel.abilityName = abilityName;
		aSel.abilityIndex = abilityIndex;
		aSel.setAbility ();

		Toggle t = inst.GetComponent<Toggle> ();
		t.group = tg;

		return aSel;
	}

	/* Instance Methods */
	public void Awake()
	{
		if (abilityIndex >= ABIL_INDEX_MAX)
			Debug.LogError ("Invalid ability index on " + name + "!");
	}

	public void Start()
	{
		toggle.onValueChanged.AddListener (toggleChanged);
	}

	#if UNITY_EDITOR
	public void Update()
	{
		setAbility ();
	}
	#endif

	private void setAbility()
	{
		ability = Ability.get (abilityName);
		if (ability != null)
		{
			abilIcon.sprite = ability.icon;
		}
	}

	public void setToggle(bool v)
	{
		toggle.isOn = v;
	}

	private void toggleChanged(bool v)
	{
		#if UNITY_EDITOR
		if(!EditorApplication.isPlaying)
			return;
		#endif
		Entity player = HUDManager.instance.getSubject ();
		if (v)
		{
			ability = Ability.get (abilityName);
			if (player.getAbility (abilityIndex) != null)
				player.swapAbility (ability, abilityIndex);
			else
				player.addAbility (ability, abilityIndex);
			Debug.Log ("Adding " + abilityName); //DEBUG ability added
		}
		else
		{
			player.removeAbility (abilityIndex);
			Debug.Log("Removing " + abilityName); //DEBUG ability removed
		}
	}
}
