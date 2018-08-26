using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MenuManager
{
	#region STATIC_VARS

	private static HUDManager instance;
	public static HUDManager GetInstance()
	{
		if (instance == null)
		{
			GameObject pref = ABU.LoadAsset<GameObject> ("core", "HUD");
			instance = Instantiate<GameObject> (pref).GetComponent<HUDManager>();
		}
		return instance;
	}
	#endregion

	#region INSTANCE_VARS

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

	#endregion


	#region INSTANCE_METHODS

	public void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
			Destroy (gameObject);

		SetSubject (null);
	}

	public void Update()
	{
		if (subject == null)
			return;

		healthBar.fillAmount = subject.HealthPerc;
		shieldBar.fillAmount = subject.ShieldPerc;

		if (Input.GetKeyDown (KeyCode.Tab)) //TODO swap for proper bindings later
		{
			if (currentMenu.name != "Ability Select Menu")
				ShowMenu (abilitySelectMenu);
			else
				ShowMenu (defaultMenu);
		}
	}

	// Change the Entity from which the HUD will pull values
	public void SetSubject(Entity subject)
	{
		//clear old subject data
		if (subject != null)
		{
			//clear old misc
			subject.damageTypeChanged -= ChangeDamageType;

			//clear old ability list
			subject.abilityAdded -= AddAbility;
			subject.abilityRemoved -= RemoveAbility;
			subject.abilitySwapped -= SwapAbilities;

			for (int i = 0; i < abilListRoot.childCount; i++)
				Destroy (abilListRoot.GetChild (i).gameObject);

			//clear old status list
			for (int i = 0; i < statListRoot.childCount; i++)
				Destroy (statListRoot.GetChild (i).gameObject);

			subject.statusAdded -= AddStatus;
			subject.statusRemoved -= RemoveStatus;
		}

		//set up new subject
		this.subject = subject;
		if (subject == null)
			return;

		//setup ability list
		for (int i = 0; i < subject.AbilityCap; i++)
		{
			if (subject.GetAbility (i) != null)
				AbilityDisplay.Create (abilListRoot, subject.GetAbility (i));
		}

		subject.abilityAdded += AddAbility;
		subject.abilityRemoved += RemoveAbility;
		subject.abilitySwapped += SwapAbilities;

		//setup status list
		foreach (Status s in subject.GetStatusList())
			AddStatus (s);

		subject.statusAdded += AddStatus;
		subject.statusRemoved += RemoveStatus;

		//misc
		subject.damageTypeChanged += ChangeDamageType;
		ChangeDamageType (subject.DefaultDT);

		//TODO other clearing and resetting of elements?
	}
	public Entity GetSubject()
	{
		return subject;
	}

	public void DisplayTextPrompt(string title, string caption, float duration = 5f)
	{
		TextPrompt.Create (GetComponent<RectTransform>(), title, caption, duration);
	}

	private void AddAbility(Ability a, int index)
	{
		AbilityDisplay ad = AbilityDisplay.Create (abilListRoot, a);
		ad.ChangeCDColor (Bullet.DamageTypeToColor (subject.DefaultDT));
		ad.transform.SetSiblingIndex (index);
	}

	private void RemoveAbility(Ability a, int index)
	{
		AbilityDisplay ad;
		for (int i = 0; i < abilListRoot.childCount; i++)
		{
			ad = abilListRoot.GetChild (i).GetComponent<AbilityDisplay> ();
			if (ad.HasAbility (a))
			{
				Destroy (ad.gameObject);
				return;
			}
		}
	}

	private void SwapAbilities(Ability a, Ability old, int index)
	{
		abilListRoot.GetChild (index).GetComponent<AbilityDisplay> ().SetSubject (a);
	}

	private void AddStatus (Status s)
	{
		StatusDisplay.create (statListRoot, s);
	}

	private void RemoveStatus (Status s)
	{
		StatusDisplay sd;
		for (int i = 0; i < statListRoot.childCount; i++)
		{
			sd = statListRoot.GetChild (i).GetComponent<StatusDisplay> ();
			if (sd.HasStatus (s))
			{
				Destroy (sd.gameObject);
				return;
			}
		}
	}

	private void ChangeDamageType(DamageType dt)
	{
		for (int i = 0; i < abilListRoot.childCount; i++)
		{
			AbilityDisplay ad = abilListRoot.GetChild (i).GetComponent<AbilityDisplay> ();
			if (ad != null)
				ad.ChangeCDColor (Bullet.DamageTypeToColor (dt));
		}
	}
	#endregion
}
