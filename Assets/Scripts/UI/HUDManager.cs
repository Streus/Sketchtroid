using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
	/* Static Vars */
	private static HUDManager _instance;
	public static HUDManager instance { get { return _instance; } }

	/* Instance Vars */

	// The Entity from which the HUD will pull values.
	[SerializeField] //DEBUG
	private Entity subject;

	// Various UI elements that correspond to data in the subject entity
	[SerializeField]
	private Image healthBar;
	[SerializeField]
	private Image shieldBar;
	[SerializeField]
	private RectTransform abilListRoot;
	//TODO add more fields to the HUDManager


	/* Instance Methods */
	public void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
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
	}

	public void setSubject(Entity subject)
	{
		//clear old subject data
		if (subject != null)
		{
			//clear old ability list
			subject.abilityAdded -= addAbility;
			subject.abilityRemoved -= removeAbility;
			subject.abilitySwapped -= swapAbilities;

			for (int i = 0; i < abilListRoot.childCount; i++)
				Destroy (abilListRoot.GetChild (i).gameObject);
		}

		//set up new subject
		this.subject = subject;
		if (subject == null)
			return;

		//setup ability list
		for (int i = 0; i < subject.abilityCount; i++)
			AbilityDisplay.create (abilListRoot, subject.getAbility (i));

		subject.abilityAdded += addAbility;
		subject.abilityRemoved += removeAbility;
		subject.abilitySwapped += swapAbilities;

		//TODO other clearing and resetting of elements?
	}

	private void addAbility(Ability a)
	{
		AbilityDisplay.create (abilListRoot, a);
	}

	private void removeAbility(Ability a)
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
}
