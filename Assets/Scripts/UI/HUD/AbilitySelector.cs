using UnityEngine;
using System.Collections;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class AbilitySelector : MonoBehaviour
{
	private const string PREF_DIR = "core";

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

	private bool activeSet;

	/* Static Methods */
	public static AbilitySelector Create(Transform parent, string abilityName, int abilityIndex, ToggleGroup tg)
	{
		GameObject pref = ABU.LoadAsset<GameObject> (PREF_DIR, "AbilityToggle");
		GameObject inst = Instantiate<GameObject> (pref, parent, false);
		AbilitySelector aSel = inst.GetComponent<AbilitySelector> ();
		aSel.abilityName = abilityName;
		aSel.abilityIndex = abilityIndex;
		aSel.SetAbility ();

		aSel.activeSet = true;

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
		toggle.onValueChanged.AddListener (ToggleChanged);
	}

	#if UNITY_EDITOR
	public void Update()
	{
		SetAbility ();
	}
	#endif

	private void SetAbility()
	{
		ability = Ability.Get (abilityName);
		if (ability != null)
		{
			abilIcon.sprite = ability.icon;
		}
	}

	public void SetToggle(bool v)
	{
		toggle.isOn = v;
	}

	public void SetActive(bool active)
	{
		activeSet = active;
	}
	public bool IsActive()
	{
		return activeSet;
	}

	private void ToggleChanged(bool v)
	{
		#if UNITY_EDITOR
		if(!EditorApplication.isPlaying)
			return;
		#endif

		if (!activeSet)
			return;

		Entity player = HUDManager.GetInstance().GetSubject ();
		if (v)
		{
			ability = Ability.Get (abilityName);
			if (player.GetAbility (abilityIndex) != null)
				player.SwapAbility (ability, abilityIndex);
			else
			{
				if (abilityIndex < 0 || abilityIndex >= player.AbilityCap)
					player.AddAbility (ability);
				else
					player.AddAbility (ability, abilityIndex);
			}
			Debug.Log ("Adding " + abilityName); //DEBUG ability added
		}
		else
		{
			player.RemoveAbility (abilityIndex);
			Debug.Log("Removing " + abilityName); //DEBUG ability removed
		}

		//DEBUG
		Debug.Log("Ability List");
		for (int i = 0; i < player.AbilityCap; i++)
		{
			Ability a = player.GetAbility (i);
			if (a != null)
				Debug.Log (i + ": " + player.GetAbility (i).name);
			else
				Debug.Log (i + ": NULL");
		}
	}
}
