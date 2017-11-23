using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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


	/* Instance Methods */
	public void Awake()
	{
		toggle.onValueChanged.AddListener (toggleChanged);
		if (abilityIndex >= ABIL_INDEX_MAX)
			Debug.LogError ("Invalid ability index on " + name + "!");
	}

	#if UNITY_EDITOR
	public void Update()
	{
		ability = Ability.get (abilityName);
		if (ability != null)
		{
			abilIcon.sprite = ability.icon;
		}
	}
	#endif

	public void setToggle(bool v)
	{
		toggle.isOn = v;
	}

	private void toggleChanged(bool v)
	{
		Entity player = HUDManager.instance.getSubject ();
		if (v)
		{
			if (player.getAbility (abilityIndex) != null)
				player.swapAbility (ability, abilityIndex);
			else
				player.addAbility (ability, abilityIndex);
		}
		else
		{
			player.removeAbility (ability);
		}
	}
}
