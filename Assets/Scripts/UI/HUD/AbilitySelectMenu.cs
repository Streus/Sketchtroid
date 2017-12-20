using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySelectMenu : MonoBehaviour
{
	/* Instance Vars */
	[SerializeField]
	private Transform offensiveLayout;
	[SerializeField]
	private string[] offensiveAbilities;
	[SerializeField]
	private Transform mobilityLayout;
	[SerializeField]
	private string[] mobilityAbilities;
	[SerializeField]
	private Transform utilityLayout;
	[SerializeField]
	private string[] utilityAbilities;

	/* Instance Methods */
	public void Awake()
	{

	}

	public void Start()
	{
		Menu m = GetComponent<Menu> ();
		if (m != null)
			m.changedFocus += focusMenu;
	}

	private void focusMenu(bool inFocus)
	{
		int i;
		if (inFocus)
		{
			ToggleGroup tg = offensiveLayout.GetComponent<ToggleGroup> ();
			for (i = 0; i < offensiveAbilities.Length; i++)
			{
				if (GameManager.instance.isAbilityUnlocked (offensiveAbilities [i]))
				{
					if(HUDManager.instance.getSubject().getAbility(0).name == offensiveAbilities [i])
						AbilitySelector.create (offensiveLayout, offensiveAbilities [i], 0, tg);
				}
			}
			
			tg = mobilityLayout.GetComponent<ToggleGroup> ();
			for (i = 0; i < mobilityAbilities.Length; i++)
			{
				if (GameManager.instance.isAbilityUnlocked (mobilityAbilities [i]))
				{
					if(HUDManager.instance.getSubject().getAbility(1).name == mobilityAbilities [i])
						AbilitySelector.create (mobilityLayout, mobilityAbilities [i], 1, tg);
				}
			}

			tg = utilityLayout.GetComponent<ToggleGroup> ();
			for (i = 0; i < utilityAbilities.Length; i++)
			{
				if (GameManager.instance.isAbilityUnlocked (utilityAbilities [i]))
				{
					if(HUDManager.instance.getSubject().getAbility(2).name == utilityAbilities [i])
						AbilitySelector.create (utilityLayout, utilityAbilities [i], 2, tg);
				}
			}
		}
		else
		{
			for(i = 0; i < offensiveLayout.transform.childCount; i++)
				Destroy(offensiveLayout.transform.GetChild(i).gameObject);
			for(i = 0; i < mobilityLayout.transform.childCount; i++)
				Destroy(mobilityLayout.transform.GetChild(i).gameObject);
			for(i = 0; i < utilityLayout.transform.childCount; i++)
				Destroy(utilityLayout.transform.GetChild(i).gameObject);
		}
	}
}
