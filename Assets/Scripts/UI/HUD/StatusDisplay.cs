using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatusDisplay : MonoBehaviour
{
	#region INSTANCE_VARS

	[SerializeField]
	private Image icon, durationIndicator;

	private Status subject;
	#endregion

	#region STATIC_METHODS

	public static StatusDisplay create(RectTransform parent, Status subject)
	{
		GameObject pref = ABU.LoadAsset<GameObject> ("core", "StatusDisplay");
		GameObject inst = Instantiate<GameObject> (pref, parent, false);
		StatusDisplay sd = inst.GetComponent<StatusDisplay> ();

		sd.SetSubject (subject);

		return sd;
	}
	#endregion

	#region INSTANCE_METHODS

	public void Update()
	{
		if (subject != null)
		{
			durationIndicator.fillAmount = subject.DurationPercentage;
		}
	}

	public void SetSubject(Status s)
	{
		subject = s;
		if (subject != null)
			icon.sprite = s.icon;
		else
			icon.sprite = null;
	}

	public bool HasStatus(Status s)
	{
		return subject == s;
	}
	#endregion
}

