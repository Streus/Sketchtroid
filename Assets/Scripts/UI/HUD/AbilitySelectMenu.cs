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
		if (inFocus)
		{
			setupWheel (0, offensiveLayout, offensiveAbilities);
			setupWheel (1, mobilityLayout, mobilityAbilities);
			setupWheel (2, utilityLayout, utilityAbilities);
		}
		else
		{
			int i;
			for(i = 0; i < offensiveLayout.transform.childCount; i++)
				Destroy(offensiveLayout.transform.GetChild(i).gameObject);
			for(i = 0; i < mobilityLayout.transform.childCount; i++)
				Destroy(mobilityLayout.transform.GetChild(i).gameObject);
			for(i = 0; i < utilityLayout.transform.childCount; i++)
				Destroy(utilityLayout.transform.GetChild(i).gameObject);
		}
	}

	private void setupWheel(int abilityIndex, Transform layout, string[] abilityList)
	{
		ToggleGroup tg = layout.GetComponent<ToggleGroup> ();
		for (int i = 0; i < abilityList.Length; i++)
		{
			if (true/*DEBUG*/ || GameManager.instance.isAbilityUnlocked (abilityList [i]))
			{
				Ability a = HUDManager.getInstance().getSubject ().getAbility (abilityIndex);
				AbilitySelector aSel = AbilitySelector.create (layout, abilityList [i], abilityIndex, tg);
				if (a != null && a.name == abilityList [i])
				{
					aSel.setActive (false);
					aSel.setToggle (true);
					aSel.setActive (true);
				}
			}
		}
	}
}
