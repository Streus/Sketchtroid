using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityDisplay : MonoBehaviour
{
	/* Instance Vars */
	[SerializeField]
	private Image cdIndicator, icon;

	private Ability subject;

	/* Static Methods */
	public static AbilityDisplay create(RectTransform parent, Ability subject)
	{
		GameObject pref = Resources.Load<GameObject> ("Prefabs/UI/HUD/AbilityDisplay");
		GameObject inst = Instantiate<GameObject> (pref, parent, false);

		AbilityDisplay ad = inst.GetComponent<AbilityDisplay> ();
		ad.setSubject (subject);

		return ad;
	}

	/* Instance Methods */
	public void Awake()
	{
		subject = null;
		if (cdIndicator == null || icon == null)
			enabled = false;
	}

	public void Update()
	{
		cdIndicator.fillAmount = subject.cooldownPercentage ();
	}

	public void setSubject(Ability subject)
	{
		this.subject = subject;
		icon.sprite = subject.icon;
	}

	public void changeCDColor(Color c)
	{
		cdIndicator.color = c;
	}

	public bool hasAbility(Ability a)
	{
		return a == subject;
	}
}
