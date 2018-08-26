using UnityEngine;
using UnityEngine.UI;

public class AbilityDisplay : MonoBehaviour
{
	private const string PREF_DIR = "core";

	private const float CD_THRESH = 0.25f;

	/* Instance Vars */
	[SerializeField]
	private Image cdIndicator, icon;
	[SerializeField]
	private Transform chargeList;

	private Ability subject;

	/* Static Methods */
	public static AbilityDisplay Create(RectTransform parent, Ability subject)
	{
		GameObject pref = ABU.LoadAsset<GameObject> (PREF_DIR, "AbilityDisplay");
		GameObject inst = Instantiate<GameObject> (pref, parent, false);

		AbilityDisplay ad = inst.GetComponent<AbilityDisplay> ();
		ad.SetSubject (subject);

		//fill charge list
		GameObject c_pref = ABU.LoadAsset<GameObject>(PREF_DIR, "ChargeIndicator");
		for (int i = 0; i < subject.ChargesMax; i++)
		{
			GameObject c_inst = Instantiate<GameObject> (c_pref, ad.chargeList, false);
			Image icon = c_inst.GetComponent<Image> ();
			if (i < subject.Charges)
				icon.fillAmount = 1f;
			else if (i == subject.Charges)
				icon.fillAmount = 1 - subject.CooldownPercentage;
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
		if (subject.Charges == 0)
			cdIndicator.fillAmount = subject.CooldownPercentage;
		//charge accumulation
		else
		{
			for (int i = 0; i < chargeList.childCount; i++)
			{
				Image icon = chargeList.GetChild (i).GetComponent<Image> ();
				if (i < subject.Charges || subject.CooldownMax > CD_THRESH)
				{
					icon.fillAmount = 1f;
				}
				else if (i == subject.Charges)
				{
					icon.fillAmount = 1 - subject.CooldownPercentage;
				}
				else
				{
					icon.fillAmount = 0f;
				}
			}
		}
	}

	public void SetSubject(Ability subject)
	{
		this.subject = subject;
		if (subject != null)
			icon.sprite = subject.icon;
		else
			icon.sprite = null;
	}

	public void ChangeCDColor(Color c)
	{
		cdIndicator.color = c;
		for (int i = 0; i < chargeList.childCount; i++)
		{
			Image icon = chargeList.GetChild (i).GetComponent<Image> ();
			if(icon != null)
				icon.color = c;
		}
	}

	public bool HasAbility(Ability a)
	{
		return a == subject;
	}
}
