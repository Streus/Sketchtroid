﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityDisplay : MonoBehaviour
{
	private const string PREF_DIR = "core";

	/* Instance Vars */
	[SerializeField]
	private Image cdIndicator, icon;
	[SerializeField]
	private Transform chargeList;

	private Ability subject;

	/* Static Methods */
	public static AbilityDisplay create(RectTransform parent, Ability subject)
	{
		GameObject pref = AssetBundleUtil.loadAsset<GameObject> (PREF_DIR, "AbilityDisplay");
		GameObject inst = Instantiate<GameObject> (pref, parent, false);

		AbilityDisplay ad = inst.GetComponent<AbilityDisplay> ();
		ad.setSubject (subject);

		//fill charge list
		GameObject c_pref = AssetBundleUtil.loadAsset<GameObject>(PREF_DIR, "ChargeIndicator");
		for (int i = 0; i < subject.chargesMax; i++)
		{
			GameObject c_inst = Instantiate<GameObject> (c_pref, ad.chargeList, false);
			Image icon = c_inst.GetComponent<Image> ();
			if (i < subject.charges)
				icon.fillAmount = 1f;
			else if (i == subject.charges)
				icon.fillAmount = 1 - subject.cooldownPercentage();
			else
				icon.fillAmount = 0f;
		}

		return ad;
	}

	/* Instance Methods */
	public void Awake()
	{
		subject = null;
		if (cdIndicator == null || icon == null || chargeList == null)
			enabled = false;
	}

	public void Update()
	{
		//initial cooldown
		if (subject.charges == 0)
			cdIndicator.fillAmount = subject.cooldownPercentage ();
		//charge accumulation
		else
		{
			for (int i = 0; i < chargeList.childCount; i++)
			{
				Image icon = chargeList.GetChild (i).GetComponent<Image> ();
				if (i < subject.charges)
				{
					icon.fillAmount = 1f;
				}
				else if (i == subject.charges)
				{
					icon.fillAmount = 1 - subject.cooldownPercentage ();
				}
				else
				{
					icon.fillAmount = 0f;
				}
			}
		}
	}

	public void setSubject(Ability subject)
	{
		this.subject = subject;
		icon.sprite = subject.icon;
	}

	public void changeCDColor(Color c)
	{
		cdIndicator.color = c;
		for (int i = 0; i < chargeList.childCount; i++)
		{
			Image icon = chargeList.GetChild (i).GetComponent<Image> ();
			if(icon != null)
				icon.color = c;
		}
	}

	public bool hasAbility(Ability a)
	{
		return a == subject;
	}
}
