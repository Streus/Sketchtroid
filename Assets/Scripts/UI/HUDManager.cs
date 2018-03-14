﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MenuManager
{
	/* Static Vars */
	private static HUDManager instance;
	public static HUDManager getInstance()
	{
		if (instance == null)
		{
			GameObject pref = AssetBundleUtil.loadAsset<GameObject> ("core/prefabs/ui", "HUD");
			instance = Instantiate<GameObject> (pref).GetComponent<HUDManager>();
		}
		return instance;
	}

	/* Instance Vars */

	// The Entity from which the HUD will pull values.
	private Entity subject;

	// Menus swapped out within the HUD
	[SerializeField]
	private Menu defaultMenu;
	[SerializeField]
	private Menu abilitySelectMenu;

	// Various UI elements that correspond to data in the subject entity
	[SerializeField]
	private Image healthBar;
	[SerializeField]
	private Image shieldBar;
	[SerializeField]
	private RectTransform abilListRoot;
	[SerializeField]
	private RectTransform statListRoot;
	//TODO add more fields to the HUDManager


	/* Instance Methods */
	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
			Destroy (gameObject);

		setSubject (null);
	}

	public void Update()
	{
		if (subject == null)
			return;

		healthBar.fillAmount = subject.healthPerc;
		shieldBar.fillAmount = subject.shieldPerc;

		if (Input.GetKeyDown (KeyCode.Tab)) //TODO swap for proper bindings later
		{
			if (currentMenu.name != "Ability Select Menu")
				showMenu (abilitySelectMenu);
			else
				showMenu (defaultMenu);
		}
	}

	// Change the Entity from which the HUD will pull values
	public void setSubject(Entity subject)
	{
		//clear old subject data
		if (subject != null)
		{
			//clear old misc
			subject.damageTypeChanged -= changeDamageType;

			//clear old ability list
			subject.abilityAdded -= addAbility;
			subject.abilityRemoved -= removeAbility;
			subject.abilitySwapped -= swapAbilities;

			for (int i = 0; i < abilListRoot.childCount; i++)
				Destroy (abilListRoot.GetChild (i).gameObject);


			for (int i = 0; i < statListRoot.childCount; i++)
				Destroy (statListRoot.GetChild (i).gameObject);
		}

		//set up new subject
		this.subject = subject;
		if (subject == null)
			return;

		//setup ability list
		for (int i = 0; i < subject.abilityCap; i++)
		{
			if (subject.getAbility (i) != null)
				AbilityDisplay.create (abilListRoot, subject.getAbility (i));
		}

		subject.abilityAdded += addAbility;
		subject.abilityRemoved += removeAbility;
		subject.abilitySwapped += swapAbilities;

		//misc
		subject.damageTypeChanged += changeDamageType;
		changeDamageType (subject.defaultDT);

		//TODO other clearing and resetting of elements?
	}
	public Entity getSubject()
	{
		return subject;
	}

	public void displayTextPrompt(string title, string caption, float duration = 5f)
	{
		TextPrompt.create (GetComponent<RectTransform>(), title, caption, duration);
	}

	private void addAbility(Ability a, int index)
	{
		AbilityDisplay ad = AbilityDisplay.create (abilListRoot, a);
		ad.changeCDColor (Bullet.damageTypeToColor (subject.defaultDT));
		ad.transform.SetSiblingIndex (index);
	}

	private void removeAbility(Ability a, int index)
	{
		AbilityDisplay ad;
		for (int i = 0; i < abilListRoot.childCount; i++)
		{
			ad = abilListRoot.GetChild (i).GetComponent<AbilityDisplay> ();
			if (ad.hasAbility (a))
				Destroy (ad.gameObject);
		}
	}

	private void swapAbilities(Ability a, Ability old, int index)
	{
		abilListRoot.GetChild (index).GetComponent<AbilityDisplay> ().setSubject (a);
	}

	private void changeDamageType(DamageType dt)
	{
		for (int i = 0; i < abilListRoot.childCount; i++)
		{
			AbilityDisplay ad = abilListRoot.GetChild (i).GetComponent<AbilityDisplay> ();
			if (ad != null)
				ad.changeCDColor (Bullet.damageTypeToColor (dt));
		}
	}
}
