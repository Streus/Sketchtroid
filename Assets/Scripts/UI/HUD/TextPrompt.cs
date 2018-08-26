using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextPrompt : MonoBehaviour
{
	#region INSTANCE_VARS

	[SerializeField]
	private Text header;
	[SerializeField]
	private Text subText;
	#endregion

	#region STATIC_METHODS

	// Create a text prompt with a given text value and duration
	public static TextPrompt Create(RectTransform parent, string title, string caption, float duration)
	{
		GameObject pref = ABU.LoadAsset<GameObject> ("core", "TextPrompt");
		GameObject inst = Instantiate<GameObject> (pref, parent, false);
		TextPrompt tp = inst.GetComponent<TextPrompt> ();

		tp.header.text = title;
		tp.subText.text = caption;

		Destroy (inst, Mathf.Abs(duration));

		return tp;
	}
	#endregion
}
